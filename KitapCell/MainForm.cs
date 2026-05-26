using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using KitapCell.Data;
using KitapCell.Repositories;
using KitapCell.Core;
using FontAwesome.Sharp;

namespace KitapCell
{
    public partial class MainForm : Form
    {
        // Windows Dark Mode Title Bar API'leri
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        private bool isAdmin = false;
        private string currentUser = "";
        private KitapCell.Models.Book _selectedBook = null;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Windows 11 Dark Mode Title Bar'ı aktifleştir (DWMWA_USE_IMMERSIVE_DARK_MODE = 20)
            if (DwmSetWindowAttribute(this.Handle, 20, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(this.Handle, 19, new[] { 1 }, 4); // Win10 Fallback
        }

        private LibraryDbContext _dbContext;
        private BookRepository _bookRepo;
        private KitapCell.Repositories.Repository<KitapCell.Models.Category> _categoryRepo;
        private KitapCell.Repositories.Repository<KitapCell.Models.Author> _authorRepo;

        // Filtre ölçütleri
        private int? _filterCategoryId = null;
        private int? _filterAuthorId   = null;
        private System.Collections.Generic.List<KitapCell.Models.Book> _allBooks = new();

        // Sol sidebar için filtre paneli referansı
        private System.Windows.Forms.Panel pnlFilterRoot = new System.Windows.Forms.Panel();

        private ContextMenuStrip bookContextMenu;
        private readonly System.Windows.Forms.ToolTip toolTipMain = new System.Windows.Forms.ToolTip();

        public MainForm()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico")); } catch { }
            ThemeHelper.Apply(this);
            this.DoubleBuffered = true;
            // ClearType font rendering aktif et
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();
            // Sidebar Nav butonlarını her zaman 15.5F Bold yap
            var navBtns = new[] { btnNavTumKitaplar, btnNavFavoriler, btnNavSonOkunanlar, btnNavEnCokOkunanlar, btnNavUyeler, btnNavOduncler, btnNavCikis };
            foreach(var b in navBtns) {
                if (b != null) b.Font = new Font("Segoe UI", 15.5F, FontStyle.Bold);
            }
            
            pnlSidebar.SizeChanged += (s, e) => {
                UpdateSidebarLayout();
            };

            _dbContext = new LibraryDbContext();
            _bookRepo = new BookRepository(_dbContext);

            if (btnNavCikis != null) btnNavCikis.Click += BtnNavCikis_Click;
            if (btnToolProfil != null) btnToolProfil.Click += BtnToolProfil_Click;
            if (picNavProfile != null) picNavProfile.Click += BtnToolProfil_Click;

            lblUserName.Cursor = Cursors.Hand;
            lblUserName.Click += BtnToolProfil_Click;

            SetupEventHandlers();
            SetActiveNavButton(btnNavTumKitaplar);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _categoryRepo = new KitapCell.Repositories.Repository<KitapCell.Models.Category>(_dbContext);
            _authorRepo   = new KitapCell.Repositories.Repository<KitapCell.Models.Author>(_dbContext);
            
            await CheckSessionAsync();
            
            await LoadBooksFromDatabaseAsync();
            await PopulateGridBooksAsync();
            await SetupFilterPanelAsync();
        }

        private async Task CheckSessionAsync()
        {
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "login_session.txt");
                if (System.IO.File.Exists(path))
                {
                    string idStr = System.IO.File.ReadAllText(path).Trim();
                    if (int.TryParse(idStr, out int userId))
                    {
                        var userRepo = new KitapCell.Repositories.UserRepository(_dbContext);
                        var user = await userRepo.GetByIdAsync(userId);
                        if (user != null && user.IsActive)
                        {
                            Core.GlobalSession.CurrentUser = user;
                            isAdmin = user.Role == Models.UserRole.Admin;
                            currentUser = $"{user.FirstName} {user.LastName}".Trim();

                            lblUserName.Text = currentUser;
                            lblUserRole.Text = isAdmin ? "🛡️ Admin" : "👤 Üye";
                            
                            bool isSmall = pnlSidebar.Width < 120;
                            lblUserName.Visible = !isSmall;
                            lblUserRole.Visible = !isSmall;
                            btnLoginSidebar.Visible = false;
                            btnNavUyeler.Visible = isAdmin;
                            btnToolSunucu.Visible = true;

                            ApplyPermissionsToUI();
                        }
                    }
                }
            }
            catch { /* Ignore session loading errors */ }
        }

        private void SetupEventHandlers()
        {
            btnLoginSidebar.Click += (s, e) => OpenLoginForm();
            btnToolKitapEkle.Click += async (s, e) => await OpenAddBookFormAsync();
            btnToolSil.Click += (s, e) => DeleteSelectedBook();
            btnToolGoruntule.Click += (s, e) => ViewSelectedBook();
            btnToolOduncVer.Click += (s, e) => LoanSelectedBook();
            btnToolIadeAl.Click += (s, e) => ReturnSelectedBook();
            btnToolRapor.Click += (s, e) => ShowReports();
            btnNavTumKitaplar.Click += async (s, e) =>
            {
                SetActiveNavButton(btnNavTumKitaplar);
                lblPageTitle.Text = "Tüm Kitaplar";
                _activeSection = "tumkitaplar";
                HideMembersView();
                // Tüm filtreleri temizle
                _filterCategoryId = null;
                _filterAuthorId   = null;
                foreach (Control ctrl in pnlFilterRoot.Controls)
                    foreach (Control c in ctrl.Controls)
                        if (c is CheckBox chk) chk.Checked = false;
                // Veritabanından taze veri çek
                await LoadBooksFromDatabaseAsync();
            };
            btnNavFavoriler.Click    += async (s, e) => { SetActiveNavButton(btnNavFavoriler);    lblPageTitle.Text = "Favoriler";     HideMembersView(); await ShowFavoritesAsync(); };
            btnNavSonOkunanlar.Click += async (s, e) => { SetActiveNavButton(btnNavSonOkunanlar); lblPageTitle.Text = "Son Okunanlar"; _activeSection = "sonokunanlar"; HideMembersView(); await ShowRecentlyReadAsync(); };
            btnNavEnCokOkunanlar.Click += async (s, e) => { SetActiveNavButton(btnNavEnCokOkunanlar); lblPageTitle.Text = "Çok Okunanlar"; _activeSection = "encokokunanlar"; HideMembersView(); await ShowMostReadBooksAsync(); };
            btnNavUyeler.Click    += async (s, e) => { SetActiveNavButton(btnNavUyeler);    lblPageTitle.Text = "Üye Listesi";     await ShowMembersAsync(); };
            btnNavOduncler.Click  += async (s, e) => { SetActiveNavButton(btnNavOduncler);  lblPageTitle.Text = "Ödünç İşlemleri"; await ShowLoansAsync(); };
            btnToolAyarlar.Click += async (s, e) =>
            {
                var SettingsForm = new SettingsForm();
                SettingsForm.ShowDialog(this);
                // Ayarlar kapandıktan sonra listeyi yenile (toplu ekleme vb. yapıldıysa)
                await LoadBooksFromDatabaseAsync();
                await PopulateGridBooksAsync();
            };
            btnToolSunucu.Click += async (s, e) =>
            {
                if (KitapCell.Web.WebServer.IsRunning)
                {
                    await KitapCell.Web.WebServer.StopAsync();
                    btnToolSunucu.Tag = "🌐|Sunucu";
                    btnToolSunucu.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
                    btnToolSunucu.Invalidate();
                    toolTipMain.SetToolTip(btnToolSunucu, "Web sunucuyu başlatmak için tıklayın");
                }
                else
                {
                    int port = KitapCell.Core.SettingsManager.Config.WebServerPort;
                    bool ok = await KitapCell.Web.WebServer.StartAsync(port);
                    if (ok)
                    {
                        btnToolSunucu.Tag = "✅|Sunucu";
                        btnToolSunucu.BackColor = System.Drawing.Color.FromArgb(20, 40, 25);
                        btnToolSunucu.Invalidate();
                        string url = KitapCell.Web.WebServer.GetNetworkUrl();
                        toolTipMain.SetToolTip(btnToolSunucu, $"Çalışıyor: {url}\n(Durdurmak için tıklayın)");
                        // Varsayılan tarayıcıda otomatik aç
                        try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
                    }
                    else
                    {
                        MessageBox.Show("Web sunucusu başlatılamadı.\nLütfen Ayarlar > Web Sunucu bölümündeki logları kontrol edin.",
                            "Sunucu Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            };
            
            // Otomatik (canlı) filtreleme eklendi
            txtContentSearch.TextChanged += (s, e) => FilterBooks(txtContentSearch.Text);
            
            // Görünüm geçiş butonları
            btnViewList.Click += (s, e) => SwitchToListView();
            btnViewGrid.Click += (s, e) => SwitchToGridView();

            dgvBooks.CellClick += DgvBooks_CellClick;
            dgvBooks.CellDoubleClick += DgvBooks_CellDoubleClick;

            // --- Değerlendirme Butonu Tıklaması ---
            btnRateBook.Click += (s, e) => {
                if (_selectedBook == null) return;
                
                if (Core.GlobalSession.CurrentUser == null)
                {
                    MessageBox.Show("Puanlama yapmak ve inceleme yazmak için lütfen giriş yapınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rForm = new BookReviewForm(_selectedBook.Id, _selectedBook.Title);
                if (rForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Kaydedilince listeyi yenile
                    _ = LoadBooksFromDatabaseAsync();
                }
            };

            // --- Canlı (Live) Sidebar Sürükleme ---
            SetupLiveSidebarDrag();
            SetupLiveRightSidebarDrag();
            SetupGridScrollbar();

            // Hover efektleri
            StyleButtonHover(btnNavTumKitaplar);
            StyleButtonHover(btnNavFavoriler);
            StyleButtonHover(btnNavSonOkunanlar);
            StyleButtonHover(btnNavEnCokOkunanlar);
            StyleButtonHover(btnNavUyeler);
            StyleButtonHover(btnNavOduncler);
            StyleButtonHover(btnToolKitapEkle);

            SetupContextMenu();
        }

        private void SetupContextMenu()
        {
            bookContextMenu = new ContextMenuStrip();
            bookContextMenu.BackColor = Color.FromArgb(30, 35, 48);
            bookContextMenu.ForeColor = Color.White;
            bookContextMenu.RenderMode = ToolStripRenderMode.System;

            var mnuOku = new ToolStripMenuItem("📖 Oku", null, (s, e) => {
                if (_selectedBook != null && _selectedBook.HasDigitalCopy) {
                    OpenBookDigital(_selectedBook);
                } else {
                    MessageBox.Show("Bu kitabın dijital kopyası (PDF/EPUB) bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
            var mnuGoruntule = new ToolStripMenuItem("👁️ Görüntüle", null, (s, e) => ViewSelectedBook());
            var mnuOduncVer = new ToolStripMenuItem("📤 Ödünç Ver", null, (s, e) => LoanSelectedBook());
            var mnuIadeAl = new ToolStripMenuItem("📥 İade Al", null, (s, e) => ReturnSelectedBook());
            var mnuSil = new ToolStripMenuItem("🗑️ Sil", null, (s, e) => DeleteSelectedBook());

            var mnuFavori = new ToolStripMenuItem("❤️ Favorilere Ekle", null, async (s, e) => {
                if (_selectedBook == null) return;
                var u2 = Core.GlobalSession.CurrentUser;
                if (u2 == null) { MessageBox.Show("Favori eklemek için giriş yapınız.", "Giriş Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                bool added = await _bookRepo.ToggleFavoriteAsync(u2.Id, _selectedBook.Id);
                string msg = added ? $"'{_selectedBook.Title}' favorilere eklendi! ❤️" : $"'{_selectedBook.Title}' favorilerden çıkarıldı.";
                MessageBox.Show(msg, "Favori", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            var mnuYorum = new ToolStripMenuItem("⭐ Yorumla / Değerlendirme Yap", null, (s, e) => {
                if (_selectedBook == null) return;
                if (Core.GlobalSession.CurrentUser == null)
                {
                    MessageBox.Show("Değerlendirme yapmak için lütfen giriş yapınız.", "Giriş Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var rForm = new BookReviewForm(_selectedBook.Id, _selectedBook.Title);
                rForm.ShowDialog(this);
            });

            bookContextMenu.Items.Add(mnuFavori);
            bookContextMenu.Items.Add(mnuYorum);
            bookContextMenu.Items.Add(new ToolStripSeparator());
            bookContextMenu.Items.Add(mnuOku);
            bookContextMenu.Items.Add(mnuGoruntule);
            bookContextMenu.Items.Add(new ToolStripSeparator());
            bookContextMenu.Items.Add(mnuOduncVer);
            bookContextMenu.Items.Add(mnuIadeAl);
            bookContextMenu.Items.Add(new ToolStripSeparator());
            bookContextMenu.Items.Add(mnuSil);

            bookContextMenu.Opening += async (s, e) => {
                // Eğer seçili kitap yoksa menüyü açma
                if (_selectedBook == null) { e.Cancel = true; return; }

                var u = Core.GlobalSession.CurrentUser;
                bool admin = u != null && u.Role == Models.UserRole.Admin;
                mnuSil.Enabled = admin || (u != null && u.CanDeleteBook);
                mnuOduncVer.Visible = _selectedBook.AvailableCopies > 0;
                mnuIadeAl.Visible = true;
                mnuOku.Visible = _selectedBook.HasDigitalCopy;

                // Favori durumunu kontrol et
                if (u != null)
                {
                    bool isFav = await _bookRepo.IsFavoriteAsync(u.Id, _selectedBook.Id);
                    mnuFavori.Text = isFav ? "🧑 Favorilerden Çıkar" : "❤️ Favorilere Ekle";
                    mnuFavori.Visible = true;
                }
                else
                {
                    mnuFavori.Visible = false;
                }
            };

            // DataGridView'de sağ tık: CellMouseDown ile satırı seç, ardından ContextMenu açılsın
            dgvBooks.ContextMenuStrip = bookContextMenu;
            dgvBooks.CellMouseDown += (s, e) => {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dgvBooks.ClearSelection();
                    dgvBooks.Rows[e.RowIndex].Selected = true;
                    if (dgvBooks.Rows[e.RowIndex].Tag is KitapCell.Models.Book book)
                    {
                        _selectedBook = book;
                        // Sağ sidebar'ı güncelle ama focus'u kaybetme
                        lblDetailTitle.Text  = book.Title;
                        lblDetailAuthor.Text = "Yazar: " + (book.Author?.FullName ?? "Bilinmiyor");
                        string pdfStatus = book.HasDigitalCopy ? "Var" : "Yok";
                        string categoryName = book.Category?.Name ?? "-";
                        lblDetailInfo.Text = $"Kategori: {categoryName}\nDijital Kopya: {pdfStatus}";
                        pnlRightSidebar.Visible = true;
                        pnlRightResizeHandle.Visible = true;
                    }
                }
            };
        }

        // Sidebar kapanma eşiği (px)
        private const int SIDEBAR_COLLAPSE_THRESHOLD = 40;
        private const int SIDEBAR_DEFAULT_WIDTH = 240;
        private bool sidebarCollapsed = false;

        private Panel pnlGridWrapper;

        private void SetupGridScrollbar()
        {
            pnlGridWrapper = new Panel();
            pnlGridWrapper.BackColor = flpGridBooks.BackColor;
            pnlGridWrapper.Dock = DockStyle.Fill;
            pnlGridWrapper.Visible = flpGridBooks.Visible;
            
            pnlContent.Controls.Remove(flpGridBooks);
            pnlContent.Controls.Add(pnlGridWrapper);
            pnlGridWrapper.BringToFront();

            flpGridBooks.Visible = true; // İçerideki asıl liste her zaman görünür olmalı
            flpGridBooks.Dock = DockStyle.None;
            flpGridBooks.AutoScroll = true;

            pnlGridWrapper.Controls.Add(flpGridBooks);

            pnlGridWrapper.Resize += (s, e) => {
                flpGridBooks.Location = new Point(0, 0);
                flpGridBooks.Size = new Size(pnlGridWrapper.Width + 25, pnlGridWrapper.Height);
            };

            pnlGridWrapper.MouseEnter += (s, e) => pnlGridWrapper.Focus();
        }

        private void SwitchToListView()
        {
            btnViewList.BackColor = Color.FromArgb(99, 102, 241);
            btnViewGrid.BackColor = Color.FromArgb(35, 40, 58);
            dgvBooks.Visible = true;
            if (pnlGridWrapper != null) pnlGridWrapper.Visible = false;
        }

        private async void SwitchToGridView()
        {
            btnViewGrid.BackColor = Color.FromArgb(99, 102, 241);
            btnViewList.BackColor = Color.FromArgb(35, 40, 58);
            dgvBooks.Visible = false;
            if (pnlGridWrapper != null) pnlGridWrapper.Visible = true;
            if (flpGridBooks.Controls.Count == 0) await PopulateGridBooksAsync();
        }

        private async Task PopulateGridBooksAsync()
        {
            flpGridBooks.Controls.Clear();

            var books = await _bookRepo.GetAllWithDetailsAsync();

            if (!books.Any())
            {
                var lblEmpty = new Label { Text = "Sistemde henüz kitap yok. Sol üstten 'Kitap Ekle' ile başlayın.", ForeColor = Color.White, AutoSize = true, Margin = new Padding(20) };
                flpGridBooks.Controls.Add(lblEmpty);
                return;
            }

            foreach(var item in books)
            {
                var pnl = new Panel { Width = 160, Height = 220, BackColor = Color.FromArgb(22, 27, 34), Margin = new Padding(15) };
                
                Control headerElement;
                if (!string.IsNullOrEmpty(item.CoverImagePath) && System.IO.File.Exists(item.CoverImagePath))
                {
                    headerElement = new PictureBox { ImageLocation = item.CoverImagePath, SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Top, Height = 130 };
                }
                else
                {
                    headerElement = new Label { Text = "📘", Font = new Font("Segoe UI", 48), ForeColor = Color.FromArgb(99,102,241), Dock = DockStyle.Top, Height = 130, TextAlign = ContentAlignment.MiddleCenter };
                }
                
                var lblTitle = new Label { Text = item.Title, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10, 5, 10, 0) };
                
                var lblAuthor = new Label { Text = item.Author?.FullName ?? "Bilinmiyor", Font = new Font("Segoe UI", 9F, FontStyle.Regular), ForeColor = Color.FromArgb(139, 148, 158), Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10, 0, 10, 0) };
                
                // Elemanları ekleme sırası Dock düzeni açısından önemlidir
                pnl.Controls.Add(lblAuthor);
                pnl.Controls.Add(lblTitle);
                pnl.Controls.Add(headerElement);
                
                EventHandler onClick = (s, e) => ShowRightSidebar(item);
                pnl.Click += onClick;
                headerElement.Click += onClick;
                lblTitle.Click += onClick;
                lblAuthor.Click += onClick;
                
                EventHandler onDoubleClick = (s, e) => {
                    if (item.HasDigitalCopy)
                    {
                        OpenBookDigital(item);
                    }
                };
                pnl.DoubleClick += onDoubleClick;
                headerElement.DoubleClick += onDoubleClick;
                lblTitle.DoubleClick += onDoubleClick;
                lblAuthor.DoubleClick += onDoubleClick;
                
                // Hover Effects for Grid item
                pnl.MouseEnter += (s, e) => pnl.BackColor = Color.FromArgb(31, 36, 44);
                pnl.MouseLeave += (s, e) => pnl.BackColor = Color.FromArgb(22, 27, 34);
                headerElement.MouseEnter += (s, e) => pnl.BackColor = Color.FromArgb(31, 36, 44);
                lblTitle.MouseEnter += (s, e) => pnl.BackColor = Color.FromArgb(31, 36, 44);
                lblAuthor.MouseEnter += (s, e) => pnl.BackColor = Color.FromArgb(31, 36, 44);
                
                // Context Menu ve Right Click
                MouseEventHandler onMouseDown = (s, e) => {
                    if (e.Button == MouseButtons.Right) {
                        ShowRightSidebar(item);
                    }
                };
                pnl.MouseDown += onMouseDown;
                headerElement.MouseDown += onMouseDown;
                lblTitle.MouseDown += onMouseDown;
                lblAuthor.MouseDown += onMouseDown;

                pnl.ContextMenuStrip = bookContextMenu;
                headerElement.ContextMenuStrip = bookContextMenu;
                lblTitle.ContextMenuStrip = bookContextMenu;
                lblAuthor.ContextMenuStrip = bookContextMenu;
                
                flpGridBooks.Controls.Add(pnl);
            }
        }
        private Image CreateEmojiImage(string emoji, int size)
        {
            var icon = FontAwesome.Sharp.IconChar.BookOpen; // Default fallback
            
            switch (emoji.Trim())
            {
                case "📚": icon = FontAwesome.Sharp.IconChar.Book; break;
                case "❤️": icon = FontAwesome.Sharp.IconChar.Heart; break;
                case "🕒": icon = FontAwesome.Sharp.IconChar.Clock; break;
                case "👥": icon = FontAwesome.Sharp.IconChar.Users; break;
                case "📤": icon = FontAwesome.Sharp.IconChar.ShareFromSquare; break;
                case "📥": icon = FontAwesome.Sharp.IconChar.Download; break;
                case "🚪": icon = FontAwesome.Sharp.IconChar.SignOutAlt; break;
                case "➕": icon = FontAwesome.Sharp.IconChar.Plus; break;
                case "🗑️": icon = FontAwesome.Sharp.IconChar.TrashAlt; break;
                case "🔍": icon = FontAwesome.Sharp.IconChar.Search; break;
                case "✏️": icon = FontAwesome.Sharp.IconChar.Pen; break;
                case "⚙️": icon = FontAwesome.Sharp.IconChar.Cog; break;
                case "⭐": icon = FontAwesome.Sharp.IconChar.Star; break;
                case "🔲": icon = FontAwesome.Sharp.IconChar.ThLarge; break;
                case "☰": icon = FontAwesome.Sharp.IconChar.List; break;
                case "👤": icon = FontAwesome.Sharp.IconChar.User; break;
                case "📖": icon = FontAwesome.Sharp.IconChar.BookOpen; break;
                case "✅": icon = FontAwesome.Sharp.IconChar.CheckCircle; break;
                case "⚠️": icon = FontAwesome.Sharp.IconChar.ExclamationTriangle; break;
                case "🔒": icon = FontAwesome.Sharp.IconChar.Lock; break;
                case "🔑": icon = FontAwesome.Sharp.IconChar.Key; break;
                case "👁️": icon = FontAwesome.Sharp.IconChar.Eye; break;
                case "📊": icon = FontAwesome.Sharp.IconChar.ChartBar; break;
                // Ek emojiler - eksik olanlar eklendi
                case "💼": icon = FontAwesome.Sharp.IconChar.Briefcase; break;          // Ödünç Ver
                case "📕": icon = FontAwesome.Sharp.IconChar.BookReader; break;          // Çok Okunanlar
                case "📘": icon = FontAwesome.Sharp.IconChar.Book; break;                // Kitap
                case "🔥": icon = FontAwesome.Sharp.IconChar.Fire; break;               // Popüler
                case "📋": icon = FontAwesome.Sharp.IconChar.ClipboardList; break;      // Liste
                case "🏠": icon = FontAwesome.Sharp.IconChar.Home; break;               // Ana Sayfa
                case "🌐": icon = FontAwesome.Sharp.IconChar.Globe; break;              // Web Sunucu
            }

            // Maviye çalan güzel bir mor/lila ikon rengi default olsun, veya beyaz/gri
            // Tema olarak daha çok gri-beyaz (201, 209, 217) kullanıyoruz navigasyonda.
            var color = Color.FromArgb(201, 209, 217); 
            
            // Eğer özel favori kalp veya yıldız ise renk verelim
            if (icon == FontAwesome.Sharp.IconChar.Heart) color = Color.FromArgb(239, 68, 68);        // Red
            if (icon == FontAwesome.Sharp.IconChar.Star) color = Color.FromArgb(234, 179, 8);         // Yellow/Gold
            if (icon == FontAwesome.Sharp.IconChar.Plus) color = Color.FromArgb(34, 197, 94);         // Green
            if (icon == FontAwesome.Sharp.IconChar.TrashAlt) color = Color.FromArgb(239, 68, 68);     // Red
            if (icon == FontAwesome.Sharp.IconChar.SignOutAlt) color = Color.FromArgb(244, 114, 182); // Pink

            // Diğer buton renkleri
            if (icon == FontAwesome.Sharp.IconChar.Book) color = Color.FromArgb(99, 102, 241);        // Indigo
            if (icon == FontAwesome.Sharp.IconChar.Clock) color = Color.FromArgb(245, 158, 11);       // Orange
            if (icon == FontAwesome.Sharp.IconChar.Users) color = Color.FromArgb(6, 182, 212);        // Cyan
            if (icon == FontAwesome.Sharp.IconChar.ShareFromSquare) color = Color.FromArgb(168, 85, 247); // Purple (Ödünç Ver)
            if (icon == FontAwesome.Sharp.IconChar.Download) color = Color.FromArgb(16, 185, 129);    // Emerald Green (İade Al)
            if (icon == FontAwesome.Sharp.IconChar.Pen) color = Color.FromArgb(234, 179, 8);          // Yellow
            if (icon == FontAwesome.Sharp.IconChar.Cog) color = Color.FromArgb(148, 163, 184);        // Slate
            if (icon == FontAwesome.Sharp.IconChar.Eye) color = Color.FromArgb(56, 189, 248);         // Sky Blue
            if (icon == FontAwesome.Sharp.IconChar.ChartBar) color = Color.FromArgb(139, 92, 246);    // Violet

            return icon.ToBitmap(color, size);
        }

        private void SetupLiveSidebarDrag()
        {
            bool dragging = false;
            int dragStartX = 0;
            int dragStartSidebarW = 0;

            // Splitter özel stili
            pnlResizeHandle.BackColor = Color.FromArgb(30, 35, 48);

            pnlResizeHandle.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    dragging = true;
                    dragStartX = Cursor.Position.X;
                    dragStartSidebarW = pnlSidebar.Width;
                }
            };

            pnlResizeHandle.MouseMove += (s, e) => {
                if (!dragging) return;
                int delta = Cursor.Position.X - dragStartX;
                int newW = dragStartSidebarW + delta;

                if (newW <= SIDEBAR_COLLAPSE_THRESHOLD)
                {
                    pnlSidebar.Width = 0;
                    pnlSidebar.Visible = false;
                    sidebarCollapsed = true;
                }
                else
                {
                    newW = Math.Min(newW, SIDEBAR_DEFAULT_WIDTH);
                    newW = Math.Max(newW, 60);
                    pnlSidebar.Visible = true;
                    pnlSidebar.Width = newW;
                    sidebarCollapsed = false;
                    bool isSmall = newW < 120;
                    bool isLoggedIn = Core.GlobalSession.CurrentUser != null;
                    lblLogoSub.Visible = !isSmall;
                    
                    lblUserName.Visible = !isSmall && isLoggedIn;
                    lblUserRole.Visible = !isSmall && isLoggedIn;
                    picNavProfile.Visible = !isSmall && isLoggedIn;
                    
                    btnLoginSidebar.Text = isSmall ? "🔑" : "Giriş Yap / Kayıt Ol";
                    btnLoginSidebar.Width = isSmall ? 40 : 196;
                    btnLoginSidebar.Visible = !isLoggedIn;
                    
                    lblNavKutuphane.Visible = !isSmall;
                    lblNavYonetim.Visible = !isSmall;
                    lblStatBooks.Visible = !isSmall;
                    lblStatMembers.Visible = !isSmall;
                    lblStatLoaned.Visible = !isSmall;
                }
            };

            pnlResizeHandle.MouseUp += (s, e) => { dragging = false; };

            // Sol kenar üzerine gidince re-aç (sadece sürükleme yapılarak)
            this.MouseMove += (s, e) => {
                if (!sidebarCollapsed) return;
                // Sol kenar: x < 8 olunca splitter’ı görünür yap
                pnlResizeHandle.Visible = (e.X < 8);
            };

            // Splitter üzerinde sol tıklayıp çekildiğinde sidebar açılacak
            pnlResizeHandle.MouseEnter += (s, e) => {
                if (sidebarCollapsed)
                    pnlResizeHandle.Cursor = Cursors.Hand;
                else
                    pnlResizeHandle.Cursor = Cursors.VSplit;
            };

            pnlResizeHandle.MouseClick += (s, e) => {
                if (sidebarCollapsed)
                {
                    pnlSidebar.Width = SIDEBAR_DEFAULT_WIDTH;
                    pnlSidebar.Visible = true;
                    sidebarCollapsed = false;
                    lblLogoSub.Visible = true;
                    pnlUserArea.Visible = true;
                    lblNavKutuphane.Visible = true;
                    lblNavYonetim.Visible = true;
                    lblStatBooks.Visible = true;
                    lblStatMembers.Visible = true;
                    lblStatLoaned.Visible = true;
                }
            };
        }

        private void SetupLiveRightSidebarDrag()
        {
            bool dragging = false;
            int dragStartX = 0;
            int dragStartSidebarW = 0;

            pnlRightResizeHandle.BackColor = Color.FromArgb(30, 35, 48);

            pnlRightResizeHandle.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    dragging = true;
                    dragStartX = Cursor.Position.X;
                    dragStartSidebarW = pnlRightSidebar.Width;
                }
            };

            pnlRightResizeHandle.MouseMove += (s, e) => {
                if (!dragging) return;
                int delta = dragStartX - Cursor.Position.X;
                
                // Kapalıysa sola doğru çekilirse (-delta) açılır (Right drag = sağa gidiş)
                if (!pnlRightSidebar.Visible)
                {
                    if (delta > 10)
                    {
                        pnlRightSidebar.Width = 180;
                        pnlRightSidebar.Visible = true;
                        dragStartX = Cursor.Position.X;
                        dragStartSidebarW = 180;
                    }
                    return;
                }

                int newW = dragStartSidebarW + delta;
                if (newW <= 40)
                {
                    pnlRightSidebar.Visible = false;
                    dragging = false;
                    return;
                }

                newW = Math.Min(newW, 600);
                newW = Math.Max(newW, 40);
                pnlRightSidebar.Width = newW;
            };

            pnlRightResizeHandle.MouseUp += (s, e) => { dragging = false; };
            pnlRightResizeHandle.MouseEnter += (s, e) => {
                pnlRightResizeHandle.BackColor = Color.FromArgb(99, 102, 241);
                pnlRightResizeHandle.Cursor = Cursors.VSplit;
            };
            pnlRightResizeHandle.MouseLeave += (s, e) => pnlRightResizeHandle.BackColor = Color.FromArgb(30, 35, 48);
            
            pnlRightResizeHandle.MouseClick += (s, e) => {
                if (!pnlRightSidebar.Visible)
                {
                    pnlRightSidebar.Width = 240;
                    pnlRightSidebar.Visible = true;
                }
            };
        }

        private string _activeSection = "tumkitaplar"; // tumkitaplar | favoriler | sonokunanlar | uyeler

        // ── Üye Listesi ─────────────────────────────────────────────────────────

        private DataGridView? _dgvMembers;

        private async Task ShowMembersAsync()
        {
            _activeSection = "uyeler";

            // Kitap görünümlerini gizle
            dgvBooks.Visible = false;
            if (pnlGridWrapper != null) pnlGridWrapper.Visible = false;
            if (_dgvLoans != null) _dgvLoans.Visible = false; // Ödünçleri gizle
            if (pnlGridWrapper != null) pnlGridWrapper.Visible = false;

            // Üye tablosu yoksa oluştur, varsa güncelle
            if (_dgvMembers == null)
            {
                _dgvMembers = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.FromArgb(13, 17, 23),
                    BorderStyle = BorderStyle.None,
                    GridColor = Color.FromArgb(35, 40, 58),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeight = 40,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    EnableHeadersVisualStyles = false
                };
                _dgvMembers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 45);
                _dgvMembers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(139, 148, 158);
                _dgvMembers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                _dgvMembers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                _dgvMembers.DefaultCellStyle.BackColor = Color.FromArgb(13, 17, 23);
                _dgvMembers.DefaultCellStyle.ForeColor = Color.FromArgb(201, 209, 217);
                _dgvMembers.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
                _dgvMembers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 51, 59);
                _dgvMembers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(201, 209, 217);
                _dgvMembers.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
                _dgvMembers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 27, 34);
                _dgvMembers.RowTemplate.Height = 42;

                _dgvMembers.Columns.AddRange(
                    new DataGridViewTextBoxColumn { Name = "colMName",  HeaderText = "AD SOYAD",       FillWeight = 160 },
                    new DataGridViewTextBoxColumn { Name = "colMEmail", HeaderText = "E-POSTA",        FillWeight = 180 },
                    new DataGridViewTextBoxColumn { Name = "colMRole",  HeaderText = "ROL",            FillWeight = 70 },
                    new DataGridViewTextBoxColumn { Name = "colMReg",   HeaderText = "KAYIT TARİHİ",  FillWeight = 100 },
                    new DataGridViewTextBoxColumn { Name = "colMLoans", HeaderText = "ÖDÜNÇ",         FillWeight = 60 },
                    new DataGridViewTextBoxColumn { Name = "colMStatus",HeaderText = "DURUM",         FillWeight = 70 }
                );

                // Sağ tık menüsü (admin: aktif/pasif yap)
                var memberMenu = new ContextMenuStrip();
                memberMenu.BackColor = Color.FromArgb(30, 35, 48);
                memberMenu.ForeColor = Color.White;

                var mnuToggleActive = new ToolStripMenuItem("🔒 Pasif Yap");
                mnuToggleActive.Click += async (s2, e2) =>
                {
                    if (_dgvMembers.SelectedRows.Count == 0) return;
                    if (_dgvMembers.SelectedRows[0].Tag is not KitapCell.Models.User u) return;
                    u.IsActive = !u.IsActive;
                    var userRepo = new KitapCell.Repositories.UserRepository(_dbContext);
                    await userRepo.UpdateAsync(u);
                    await ShowMembersAsync(); // yenile
                };
                memberMenu.Opening += (s2, e2) =>
                {
                    if (_dgvMembers.SelectedRows.Count == 0) { e2.Cancel = true; return; }
                    if (_dgvMembers.SelectedRows[0].Tag is KitapCell.Models.User u)
                        mnuToggleActive.Text = u.IsActive ? "🔒 Pasif Yap" : "✅ Aktif Yap";
                };
                memberMenu.Items.Add(mnuToggleActive);
                _dgvMembers.ContextMenuStrip = memberMenu;
                _dgvMembers.CellMouseDown += (s2, e2) =>
                {
                    if (e2.Button == MouseButtons.Right && e2.RowIndex >= 0)
                    {
                        _dgvMembers.ClearSelection();
                        _dgvMembers.Rows[e2.RowIndex].Selected = true;
                    }
                };

                pnlContent.Controls.Add(_dgvMembers);
            }

            _dgvMembers.Rows.Clear();
            _dgvMembers.Visible = true;
            _dgvMembers.BringToFront();

            var userRepo2 = new KitapCell.Repositories.UserRepository(_dbContext);
            var users = (await userRepo2.GetAllWithLoansAsync()).ToList();

            foreach (var u in users)
            {
                int activeLoans = u.Loans.Count(l => l.ReturnDate == null);
                string roleText = u.Role == Models.UserRole.Admin ? "🛡️ Admin" : "👤 Üye";
                string statusText = u.IsActive ? "✅ Aktif" : "🔒 Pasif";

                int rowIdx = _dgvMembers.Rows.Add(
                    $"{u.FirstName} {u.LastName}",
                    u.Email,
                    roleText,
                    u.RegistrationDate.ToString("dd.MM.yyyy"),
                    activeLoans > 0 ? $"📤 {activeLoans}" : "—",
                    statusText
                );
                _dgvMembers.Rows[rowIdx].Tag = u;

                // Pasif üyeleri soluk göster
                if (!u.IsActive)
                    _dgvMembers.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(80, 85, 100);
            }
        }

        private void HideMembersView()
        {
            if (_dgvMembers != null) _dgvMembers.Visible = false;
            if (_dgvLoans != null) _dgvLoans.Visible = false;
            dgvBooks.Visible = true;
        }

        // ── Ödünç Listesi ─────────────────────────────────────────────────────────

        private DataGridView? _dgvLoans;

        private async Task ShowLoansAsync()
        {
            _activeSection = "oduncler";
            dgvBooks.Visible = false;
            if (pnlGridWrapper != null) pnlGridWrapper.Visible = false;
            if (_dgvMembers != null) _dgvMembers.Visible = false; // Üyeleri gizle

            if (_dgvLoans == null)
            {
                _dgvLoans = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.FromArgb(13, 17, 23),
                    BorderStyle = BorderStyle.None,
                    GridColor = Color.FromArgb(35, 40, 58),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeight = 40,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    EnableHeadersVisualStyles = false
                };
                _dgvLoans.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 45);
                _dgvLoans.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(139, 148, 158);
                _dgvLoans.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                _dgvLoans.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                _dgvLoans.DefaultCellStyle.BackColor = Color.FromArgb(13, 17, 23);
                _dgvLoans.DefaultCellStyle.ForeColor = Color.FromArgb(201, 209, 217);
                _dgvLoans.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
                _dgvLoans.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 51, 59);
                _dgvLoans.DefaultCellStyle.SelectionForeColor = Color.FromArgb(201, 209, 217);
                _dgvLoans.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
                _dgvLoans.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 27, 34);
                _dgvLoans.RowTemplate.Height = 42;

                _dgvLoans.Columns.AddRange(
                    new DataGridViewTextBoxColumn { Name = "colLBook",   HeaderText = "KİTAP ADI",      FillWeight = 180 },
                    new DataGridViewTextBoxColumn { Name = "colLUser",   HeaderText = "ÜDÜNÇ ALAN ÜYE",  FillWeight = 150 },
                    new DataGridViewTextBoxColumn { Name = "colLBorrow", HeaderText = "ALINMA TARİHİ", FillWeight = 100 },
                    new DataGridViewTextBoxColumn { Name = "colLDue",    HeaderText = "SON TESTİM",    FillWeight = 100 },
                    new DataGridViewTextBoxColumn { Name = "colLStatus", HeaderText = "DURUM",          FillWeight = 120 }
                );
                
                // Sağ tık menüsü (iade alma vb)
                var loanMenu = new ContextMenuStrip();
                loanMenu.BackColor = Color.FromArgb(30, 35, 48);
                loanMenu.ForeColor = Color.White;

                var mnuReturn = new ToolStripMenuItem("📥 İade Al");
                mnuReturn.Click += async (s2, e2) =>
                {
                    if (_dgvLoans.SelectedRows.Count == 0) return;
                    if (_dgvLoans.SelectedRows[0].Tag is not Models.BookLoan l) return;
                    
                    var uService = new Services.UserService(new Repositories.UserRepository(_dbContext));
                    var bService = new Services.BookService(new Repositories.BookRepository(_dbContext), new Repositories.LoanRepository(_dbContext), uService);
                    
                    var result = await bService.ReturnBookAsync(l.Id);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await ShowLoansAsync(); // yenile
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                loanMenu.Opening += (s2, e2) =>
                {
                    if (_dgvLoans.SelectedRows.Count == 0) { e2.Cancel = true; return; }
                    var l = _dgvLoans.SelectedRows[0].Tag as Models.BookLoan;
                    mnuReturn.Enabled = l != null && l.ReturnDate == null;
                };
                loanMenu.Items.Add(mnuReturn);
                _dgvLoans.ContextMenuStrip = loanMenu;
                _dgvLoans.CellMouseDown += (s2, e2) =>
                {
                    if (e2.Button == MouseButtons.Right && e2.RowIndex >= 0)
                    {
                        _dgvLoans.ClearSelection();
                        _dgvLoans.Rows[e2.RowIndex].Selected = true;
                    }
                };

                pnlContent.Controls.Add(_dgvLoans);
            }

            _dgvLoans.Rows.Clear();
            _dgvLoans.Visible = true;
            _dgvLoans.BringToFront();

            var loanRepo = new Repositories.LoanRepository(_dbContext);
            var loans = (await loanRepo.GetAllWithDetailsAsync()).ToList();

            foreach (var l in loans)
            {
                string statusText = l.ReturnDate.HasValue ? $"✅ İade Edildi ({l.ReturnDate.Value.ToString("dd.MM.yyyy")})" : 
                                    (l.DueDate < DateTime.Now ? "⚠️ Gecikti!" : "📤 Aktif");

                int rowIdx = _dgvLoans.Rows.Add(
                    l.Book?.Title ?? "Bilinmiyor",
                    (l.User?.FirstName + " " + l.User?.LastName) ?? "Bilinmiyor",
                    l.BorrowDate.ToString("dd.MM.yyyy"),
                    l.DueDate.ToString("dd.MM.yyyy"),
                    statusText
                );
                _dgvLoans.Rows[rowIdx].Tag = l;

                if (!l.ReturnDate.HasValue && l.DueDate < DateTime.Now)
                {
                    _dgvLoans.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(239, 68, 68); // Red
                }
                else if (l.ReturnDate.HasValue)
                {
                    _dgvLoans.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(139, 148, 158); // Gray
                }
            }
        }

        private async Task ShowFavoritesAsync()
        {
            _activeSection = "favoriler";
            var u = Core.GlobalSession.CurrentUser;
            if (u == null)
            {
                MessageBox.Show("Favorileri görmek için giriş yapınız.", "Giriş Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var books = (await _bookRepo.GetFavoritesAsync(u.Id)).ToList();
            RenderFilteredBooks(books, books.Count == 0 ? "❤️ Henüz favori kitap eklemediniz." : null);
        }

        private async Task ShowRecentlyReadAsync()
        {
            _activeSection = "sonokunanlar";
            var u = Core.GlobalSession.CurrentUser;
            if (u == null)
            {
                MessageBox.Show("Son okunanları görmek için giriş yapınız.", "Giriş Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var books = (await _bookRepo.GetRecentlyReadAsync(u.Id)).ToList();
            RenderFilteredBooks(books, books.Count == 0 ? "🕐 Henüz okunmuş kitap kaydedilmemiş." : null);
        }

        // Liste + Grid görünümünü verilen kitap listesiyle doldurur
        private void RenderFilteredBooks(System.Collections.Generic.List<KitapCell.Models.Book> books, string? emptyMessage = null)
        {
            // DataGridView
            dgvBooks.Rows.Clear();
            foreach (var book in books)
            {
                string digitalLabel2 = "📄 Dijital";
                if (book.HasDigitalCopy && !string.IsNullOrEmpty(book.PdfFilePath))
                {
                    string ext2 = System.IO.Path.GetExtension(book.PdfFilePath).ToLower();
                    digitalLabel2 = ext2 == ".epub" ? "📄 EPUB" : "📄 PDF";
                }
                string status = book.HasDigitalCopy ? digitalLabel2 : (book.AvailableCopies > 0 ? "✅ Müsait" : "📤 Ödünçte");
                int rowIdx = dgvBooks.Rows.Add(book.Title, book.Author?.FullName ?? "Bilinmiyor",
                    book.Category?.Name ?? "-", book.Publisher ?? "-",
                    book.AverageRating > 0 ? $"{book.AverageRating} / 5" : "-", status);
                dgvBooks.Rows[rowIdx].Tag = book;
            }

            // Grid (FlowLayout)
            flpGridBooks.Controls.Clear();
            if (books.Count == 0 && emptyMessage != null)
            {
                var lbl = new Label { Text = emptyMessage, ForeColor = Color.FromArgb(139, 148, 158),
                    Font = new Font("Segoe UI", 14F), AutoSize = true, Margin = new Padding(30) };
                flpGridBooks.Controls.Add(lbl);
                return;
            }
            foreach (var item in books)
            {
                var pnl = new Panel { Width = 160, Height = 220, BackColor = Color.FromArgb(22, 27, 34), Margin = new Padding(15) };
                Control headerElement;
                if (!string.IsNullOrEmpty(item.CoverImagePath) && System.IO.File.Exists(item.CoverImagePath))
                    headerElement = new PictureBox { ImageLocation = item.CoverImagePath, SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Top, Height = 130 };
                else
                    headerElement = new Label { Text = "📘", Font = new Font("Segoe UI", 48), ForeColor = Color.FromArgb(99,102,241), Dock = DockStyle.Top, Height = 130, TextAlign = ContentAlignment.MiddleCenter };
                var lblTitle = new Label { Text = item.Title, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10,5,10,0) };
                var lblAuthor = new Label { Text = item.Author?.FullName ?? "Bilinmiyor", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(139,148,158), Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10,0,10,0) };
                pnl.Controls.Add(lblAuthor); pnl.Controls.Add(lblTitle); pnl.Controls.Add(headerElement);
                var bookRef = item;
                EventHandler onClick = (s2, e2) => ShowRightSidebar(bookRef);
                pnl.Click += onClick; headerElement.Click += onClick; lblTitle.Click += onClick; lblAuthor.Click += onClick;
                MouseEventHandler onRightClick = (s2, e2) => { if (e2.Button == MouseButtons.Right) ShowRightSidebar(bookRef); };
                pnl.MouseDown += onRightClick; headerElement.MouseDown += onRightClick; lblTitle.MouseDown += onRightClick; lblAuthor.MouseDown += onRightClick;
                pnl.ContextMenuStrip = bookContextMenu; headerElement.ContextMenuStrip = bookContextMenu;
                lblTitle.ContextMenuStrip = bookContextMenu; lblAuthor.ContextMenuStrip = bookContextMenu;
                pnl.MouseEnter += (s2, e2) => pnl.BackColor = Color.FromArgb(31,36,44);
                pnl.MouseLeave += (s2, e2) => pnl.BackColor = Color.FromArgb(22,27,34);
                flpGridBooks.Controls.Add(pnl);
            }
        }

        private async Task ShowMostReadBooksAsync()
        {
            var books = (await _bookRepo.GetMostReadAsync()).ToList();
            RenderFilteredBooks(books, books.Count == 0 ? "🔥 Çok okunan kitap bulunamadı." : null);
        }

        private async void UpdateContentSearch()
        {
            if (_activeSection == "favoriler") await ShowFavoritesAsync();
            else if (_activeSection == "sonokunanlar") await ShowRecentlyReadAsync();
            else if (_activeSection == "encokokunanlar") await ShowMostReadBooksAsync();
        }

        private void SetActiveNavButton(Button active)
        {
            Button[] navBtns = { btnNavTumKitaplar, btnNavFavoriler, btnNavSonOkunanlar, btnNavEnCokOkunanlar, btnNavUyeler, btnNavOduncler, btnNavCikis };
            foreach (var btn in navBtns)
            {
                if (btn == null) continue;
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(180, 185, 215);
                // Font boyutu ve kalınlığı sidebar'da her zaman 15.5F Bold kalır
                btn.Font = new Font("Segoe UI", 15.5F, FontStyle.Bold);
            }
            // Aktif butonu renk ile vurgula — font değiştirilmiyor!
            active.BackColor = Color.FromArgb(99, 102, 241, 30);
            active.ForeColor = Color.FromArgb(150, 130, 255);
            active.Font = new Font("Segoe UI", 15.5F, FontStyle.Bold);
        }

        private void StyleButtonHover(Button btn)
        {
            var origColor = btn.BackColor;
            var origFore = btn.ForeColor;
            btn.MouseEnter += (s, e) =>
            {
                if (btn.BackColor == Color.Transparent || btn.BackColor == origColor)
                    btn.BackColor = Color.FromArgb(31, 36, 44);
            };
            btn.MouseLeave += (s, e) =>
            {
                if (btn.Tag?.ToString() != "active")
                    btn.BackColor = origColor;
            };
        }

        private async Task LoadBooksFromDatabaseAsync()
        {
            dgvBooks.Rows.Clear();
            var books = (await _bookRepo.GetAllWithDetailsAsync()).ToList();
            _allBooks = books; // Filtre için cache

            foreach (var book in books)
            {
                string digitalLabel = "📄 Dijital";
                if (book.HasDigitalCopy && !string.IsNullOrEmpty(book.PdfFilePath))
                {
                    string ext = System.IO.Path.GetExtension(book.PdfFilePath).ToLower();
                    digitalLabel = ext == ".epub" ? "📄 EPUB" : "📄 PDF";
                }
                string status = book.HasDigitalCopy
                    ? digitalLabel
                    : (book.AvailableCopies > 0 ? "✅ Müsait" : "📤 Ödünçte");
                string authorName = book.Author?.FullName ?? "Bilinmiyor";
                string categoryName = book.Category?.Name ?? "Belirtilmemiş";
                string rating = book.AverageRating > 0 ? $"{book.AverageRating} / 5" : "-";
                
                int rowIndex = dgvBooks.Rows.Add(
                    book.Title, 
                    authorName, 
                    categoryName, 
                    book.Publisher ?? "-", 
                    rating, 
                    status
                );
                dgvBooks.Rows[rowIndex].Tag = book;
            }
        }

        private void FilterBooks(string query)
        {
            bool isEmptyQuery = string.IsNullOrEmpty(query);
            string qLower = query?.ToLower() ?? "";

            // ── Liste görünümü (DataGridView) ────────────────────────────────────
            foreach (DataGridViewRow row in dgvBooks.Rows)
            {
                bool visible = isEmptyQuery;
                if (!visible)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value?.ToString().ToLower().Contains(qLower) == true)
                        { visible = true; break; }
                    }
                }
                row.Visible = visible;
            }

            // ── Kapak (Grid) görünümü (FlowLayoutPanel) ──────────────────────────
            foreach (Control ctrl in flpGridBooks.Controls)
            {
                if (ctrl is not Panel pnl) continue;

                if (isEmptyQuery)
                {
                    pnl.Visible = true;
                    continue;
                }

                bool found = false;
                foreach (Control child in pnl.Controls)
                {
                    if (child is Label lbl && !string.IsNullOrEmpty(lbl.Text)
                        && lbl.Text.ToLower().Contains(qLower))
                    {
                        found = true;
                        break;
                    }
                }
                pnl.Visible = found;
            }
        }

        private void DgvBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvBooks.Rows[e.RowIndex].Tag is KitapCell.Models.Book book)
            {
                ShowRightSidebar(book);
            }
        }

        private void DgvBooks_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvBooks.Rows[e.RowIndex].Tag is KitapCell.Models.Book book)
            {
                if (book.HasDigitalCopy)
                {
                    OpenBookDigital(book);
                }
            }
        }

        private void OpenBookDigital(KitapCell.Models.Book book)
        {
            if (book == null || !book.HasDigitalCopy) return;
            
            string ext = System.IO.Path.GetExtension(book.PdfFilePath)?.ToLower();
            if (ext == ".pdf" && SettingsManager.Config.PdfOpenMode == PdfOpenMode.System)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(book.PdfFilePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sistem PDF okuyucusu başlatılamadı:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                var readerForm = new BookReaderForm(book);
                readerForm.ShowDialog(this);
            }
        }

        private void ShowRightSidebar(KitapCell.Models.Book book)
        {
            _selectedBook = book;
            lblDetailTitle.Text = book.Title;
            lblDetailAuthor.Text = "Yazar: " + (book.Author?.FullName ?? "Bilinmiyor");
            
            string status = book.AvailableCopies > 0 ? $"✅ Müsait ({book.AvailableCopies})" : "📤 Ödünçte";
            string categoryName = book.Category?.Name ?? "-";
            string publishYear = book.PublishYear?.ToString() ?? "-";
            string publisherName = book.Publisher ?? "-";
            string isbn = string.IsNullOrEmpty(book.ISBN) ? "Yok" : book.ISBN;
            // Dijital kopya etiketini uzantıya göre belirle
            string digitalFormatLabel = "Yok";
            if (book.HasDigitalCopy && !string.IsNullOrEmpty(book.PdfFilePath))
            {
                string sideExt = System.IO.Path.GetExtension(book.PdfFilePath).ToLower();
                digitalFormatLabel = sideExt == ".epub" ? "Var (EPUB)" : "Var (PDF)";
            }

            lblDetailInfo.Text = $"Kategori: {categoryName}\nBasım Yılı: {publishYear}\nYayın Evi: {publisherName}\nISBN: {isbn}\nDijital Kopya: {digitalFormatLabel}\n\nDurum: {status}";
            
            if (!string.IsNullOrEmpty(book.CoverImagePath) && System.IO.File.Exists(book.CoverImagePath))
                picDetailCover.ImageLocation = book.CoverImagePath;
            else
                picDetailCover.Image = null;

            pnlRightSidebar.Visible = true;
            pnlRightResizeHandle.Visible = true;
            
            pnlContent.BringToFront(); 
        }

        private void OpenLoginForm()
        {
            var loginForm = new LoginForm();
            if (loginForm.ShowDialog(this) == DialogResult.OK)
            {
                isAdmin = loginForm.IsAdmin;
                currentUser = loginForm.UserName;
                lblUserName.Text = currentUser;
                lblUserRole.Text = isAdmin ? "🛡️ Admin" : "👤 Üye";
                lblUserName.Visible = true;
                lblUserRole.Visible = true;
                btnLoginSidebar.Visible = false;
                btnNavUyeler.Visible = isAdmin;
                btnToolSunucu.Visible = true;

                // Yetki bazlı toolbar butonlarını güncelle
                ApplyPermissionsToUI();
            }
        }

        private void ApplyPermissionsToUI()
        {
            var u = Core.GlobalSession.CurrentUser;
            if (u == null)
            {
                btnToolKitapEkle.Enabled = false;
                btnToolSil.Enabled = false;
                if (btnToolProfil != null) btnToolProfil.Visible = false;
                if (btnToolAyarlar != null) btnToolAyarlar.Visible = false;
                if (btnNavCikis != null) btnNavCikis.Visible = false;
                if (picNavProfile != null) picNavProfile.Visible = false;
                return;
            }

            bool admin = u.Role == Models.UserRole.Admin;

            // Admin her şeyi yapabilir, diğerleri sadece kendine tanımlanmış yetkileri kullanabilir
            btnToolKitapEkle.Enabled = admin || u.CanAddBook;
            btnToolSil.Enabled     = admin || u.CanDeleteBook;
            btnToolGoruntule.Enabled = true; // herkes görebilir
            btnToolOduncVer.Enabled  = true; // herkes ödünç işlemi yapabilir

            if (btnToolProfil != null) btnToolProfil.Visible = true;
            if (btnToolAyarlar != null) btnToolAyarlar.Visible = admin;
            if (btnNavCikis != null) btnNavCikis.Visible = true;

            if (picNavProfile != null)
            {
                picNavProfile.Visible = !sidebarCollapsed;
                if (!string.IsNullOrEmpty(u.ProfileImagePath) && System.IO.File.Exists(u.ProfileImagePath))
                {
                    try { picNavProfile.Image = Image.FromFile(u.ProfileImagePath); }
                    catch { picNavProfile.Image = null; }
                }
                else
                {
                    picNavProfile.Image = null;
                }
            }
        }

        private void BtnToolProfil_Click(object? sender, EventArgs e)
        {
            if (Core.GlobalSession.CurrentUser == null) return;
            using var profForm = new ProfileForm();
            profForm.ShowDialog(this);
            
            if (profForm.LogoutRequested)
            {
                BtnNavCikis_Click(null, EventArgs.Empty);
                return;
            }

            // Profil değiştiğinde (örn. Ad Soyad) hemen isim etiketini updatele
            lblUserName.Text = $"{Core.GlobalSession.CurrentUser.FirstName} {Core.GlobalSession.CurrentUser.LastName}".Trim();
            ApplyPermissionsToUI(); // profil fotoğrafı da güncellenir
        }

        private void BtnNavCikis_Click(object? sender, EventArgs e)
        {
            Core.GlobalSession.CurrentUser = null;
            isAdmin = false;
            currentUser = "";
            
            try 
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "login_session.txt");
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            } 
            catch { }

            lblUserName.Visible = false;
            lblUserRole.Visible = false;
            btnLoginSidebar.Visible = true;
            
            ApplyPermissionsToUI();
            btnNavTumKitaplar.PerformClick();
        }

        private async Task OpenAddBookFormAsync(bool editMode = false)
        {
            string existingTitle = "";
            if (editMode && dgvBooks.SelectedRows.Count > 0)
                existingTitle = dgvBooks.SelectedRows[0].Cells["colTitle"].Value?.ToString()?.Replace("📖  ", "") ?? "";
            
            var form = new AddBookForm(existingTitle);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // Kitap eklendiyse ekranı yenile
                await LoadBooksFromDatabaseAsync();
                await PopulateGridBooksAsync();
            }
        }

        private async void DeleteSelectedBook()
        {
            if (_selectedBook == null) { MessageBox.Show("Silinecek bir kitap seçin.", "Bilgi"); return; }
            
            if (MessageBox.Show($"'{_selectedBook.Title}' kitabını silmek istediğinizden emin misiniz?", "Sil Sistemi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await _bookRepo.DeleteAsync(_selectedBook.Id);
                
                _selectedBook = null;
                pnlRightSidebar.Visible = false;
                pnlRightResizeHandle.Visible = false;

                await LoadBooksFromDatabaseAsync();
                await PopulateGridBooksAsync();
            }
        }

        private void ViewSelectedBook()
        {
            if (_selectedBook == null) { MessageBox.Show("Görüntülenecek bir kitap seçin.", "Bilgi"); return; }
            MessageBox.Show($"Kitap: {_selectedBook.Title}\n\nDetay görüntüleme ekranı sağ taraftan açılmıştır.", "Kitap Detayı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowRightSidebar(_selectedBook);
        }

        private async void LoanSelectedBook()
        {
            if (_selectedBook == null) { MessageBox.Show("Ödünç verilecek bir kitap seçin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (_selectedBook.AvailableCopies <= 0) { MessageBox.Show("Bu kitap stokta kalmamıştır. Tüm kopyalar şu anda ödünç verilmiş durumda.", "Stok Yetersiz", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            
            var form = new LendBookForm(_selectedBook);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                pnlRightSidebar.Visible = false;
                pnlRightResizeHandle.Visible = false;
                
                // Form kapandığında gridleri tekrar yükleyelimki 'Müsait' statusu ve Adetler güncellensin
                await LoadBooksFromDatabaseAsync();
                await PopulateGridBooksAsync();
            }
        }

        private async void ReturnSelectedBook()
        {
            var form = new ReturnBookForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await LoadBooksFromDatabaseAsync();
                await PopulateGridBooksAsync();
            }
        }

        private void ShowReports()
        {
            var ReportsForm = new ReportsForm();
            ReportsForm.ShowDialog(this);
        }

        // ── Sol Sidebar Accordion Filtreler ─────────────────────────────────────

        private Panel CreateAccordionBlock(string title, System.Windows.Forms.Control[] items, ref int yOffset)
        {
            var header = new Button
            {
                Text      = $"▶  {title}",
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 185, 215),
                BackColor = Color.FromArgb(26, 30, 46),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 0, 0),
                Location  = new Point(0, yOffset),
                Size      = new Size(234, 34),
                Cursor    = Cursors.Hand
            };
            header.FlatAppearance.BorderSize = 0;

            // Her checkbox satırı ~26px, panel padding 8px
            int itemHeight = items.Length * 28 + 8;

            var itemPanel = new Panel
            {
                BackColor = Color.FromArgb(18, 22, 29),
                Location  = new Point(0, yOffset + 34),
                Size      = new Size(234, itemHeight),
                Visible   = false
            };

            int iy = 6;
            foreach (var item in items)
            {
                item.Location = new Point(12, iy);
                item.Width    = 210;
                itemPanel.Controls.Add(item);
                iy += 28;
            }

            bool expanded = false;
            header.Click += (s, e) =>
            {
                expanded          = !expanded;
                itemPanel.Visible = expanded;
                header.Text       = (expanded ? "▼" : "▶") + $"  {title}";

                // Tüm kontrollerin konumunu yeniden hesapla
                int y2 = 0;
                foreach (Control ctrl in pnlFilterRoot.Controls)
                {
                    ctrl.Location = new Point(0, y2);
                    y2 += ctrl.Visible ? ctrl.Height : (ctrl is Button ? ctrl.Height : 0);
                    // Her zaman header görünür, sadece itemPanel gizlenebilir
                }
                // Doğru hesaplama: header her zaman görünür
                y2 = 0;
                foreach (Control ctrl in pnlFilterRoot.Controls)
                {
                    if (!ctrl.Visible && ctrl is Panel) { ctrl.Location = new Point(0, y2); continue; }
                    ctrl.Location = new Point(0, y2);
                    y2 += ctrl.Height;
                }
                pnlFilterRoot.Height = y2 + 4;
                UpdateSidebarLayout();
                pnlSidebar.Invalidate();
            };

            pnlFilterRoot.Controls.Add(header);
            pnlFilterRoot.Controls.Add(itemPanel);
            yOffset += 34; // sadece header yüksekliği (itemPanel kapalı başlar)
            return itemPanel;
        }

        private CheckBox CreateFilterCheckBox(string text)
        {
            return new CheckBox
            {
                Text      = text,
                Font      = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(180, 185, 215),
                AutoSize  = true,
                Margin    = new Padding(4, 3, 0, 3),
                Cursor    = Cursors.Hand
            };
        }

        private async Task SetupFilterPanelAsync()
        {
            pnlFilterRoot.BackColor = Color.FromArgb(18, 22, 29);
            pnlFilterRoot.Width     = 234;
            pnlFilterRoot.Height    = 68; // Dinamik büyüyecek
            pnlFilterRoot.Location  = new Point(0, 412); // Ödünç İşlemleri btn altı (y=371+38+3)
            pnlFilterRoot.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            pnlSidebar.Controls.Add(pnlFilterRoot);
            pnlFilterRoot.BringToFront();

            var categories = await _categoryRepo.GetAllAsync();
            var authors    = await _authorRepo.GetAllAsync();

            int y = 0;

            // ─ Filtreler ayırıcı başlığı
            var lblFilterHeader = new Label
            {
                Text      = "  🔍 FILTRELER",
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 95, 120),
                BackColor = Color.FromArgb(18, 22, 29),
                Location  = new Point(0, y),
                Size      = new Size(234, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlFilterRoot.Controls.Add(lblFilterHeader);
            y += 26;

            // ─ Kategori accordion
            var catItems = categories.Select(cat =>
            {
                var chk = CreateFilterCheckBox(cat.Name);
                chk.CheckedChanged += (s, e) =>
                {
                    if (chk.Checked)
                    {
                        _filterCategoryId = cat.Id;
                        foreach (Control c in ((Control)s).Parent.Controls)
                            if (c is CheckBox other && other != chk) other.Checked = false;
                    }
                    else if (_filterCategoryId == cat.Id)
                        _filterCategoryId = null;
                    ApplyFilters();
                };
                return (System.Windows.Forms.Control)chk;
            }).ToArray();
            CreateAccordionBlock("Kategoriler", catItems, ref y);

            // ─ Yazar accordion
            var authorItems = authors.Select(aut =>
            {
                var chk = CreateFilterCheckBox(aut.FullName);
                chk.CheckedChanged += (s, e) =>
                {
                    if (chk.Checked)
                    {
                        _filterAuthorId = aut.Id;
                        foreach (Control c in ((Control)s).Parent.Controls)
                            if (c is CheckBox other && other != chk) other.Checked = false;
                    }
                    else if (_filterAuthorId == aut.Id)
                        _filterAuthorId = null;
                    ApplyFilters();
                };
                return (System.Windows.Forms.Control)chk;
            }).ToArray();
            CreateAccordionBlock("Yazarlar", authorItems, ref y);

            pnlFilterRoot.Height = y + 4;
            
            // pnlFilterRoot (Kategoriler vs.) Kütüphane linklerinin tam altına gelsin
            pnlFilterRoot.Location = new Point(0, 375);
            
            // Yönetim paneli linklerini pnlFilterRoot'un ne kadar yer kapladığına bağlı olarak aşağı itiyoruz
            UpdateSidebarLayout();
        }

        private void UpdateSidebarLayout()
        {
            if (pnlFilterRoot == null) return;
            
            int newYonetimY = pnlFilterRoot.Bottom + 15;
            if (pnlNavSep2 != null) pnlNavSep2.Location = new Point(12, newYonetimY);
            if (lblNavYonetim != null) lblNavYonetim.Location = new Point(20, newYonetimY + 10);
            if (btnNavUyeler != null) btnNavUyeler.Location = new Point(8, newYonetimY + 38);
            if (lblStatMembers != null) lblStatMembers.Location = new Point(188, newYonetimY + 38 + 10);
            if (btnNavOduncler != null) btnNavOduncler.Location = new Point(8, newYonetimY + 83);
            if (lblStatLoaned != null) lblStatLoaned.Location = new Point(188, newYonetimY + 83 + 10);

            if (btnNavCikis != null)
            {
                int cikisY = Math.Max(pnlSidebar.Height - 64, newYonetimY + 130);
                btnNavCikis.Location = new Point(8, cikisY);
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allBooks.AsEnumerable();

            if (_filterCategoryId.HasValue)
                filtered = filtered.Where(b => b.CategoryId == _filterCategoryId.Value);
            if (_filterAuthorId.HasValue)
                filtered = filtered.Where(b => b.AuthorId == _filterAuthorId.Value);

            var list = filtered.ToList();

            // DataGridView güncelle
            dgvBooks.Rows.Clear();
            foreach (var book in list)
            {
                string digitalLabelF = "📄 Dijital";
                if (book.HasDigitalCopy && !string.IsNullOrEmpty(book.PdfFilePath))
                {
                    string extF = System.IO.Path.GetExtension(book.PdfFilePath).ToLower();
                    digitalLabelF = extF == ".epub" ? "📄 EPUB" : "📄 PDF";
                }
                string status = book.HasDigitalCopy ? digitalLabelF : (book.AvailableCopies > 0 ? "✅ Müsait" : "📤 Ödünçte");
                int rowIdx = dgvBooks.Rows.Add(
                    book.Title,
                    book.Author?.FullName ?? "Bilinmiyor",
                    book.Category?.Name ?? "-",
                    book.Publisher ?? "-",
                    book.AverageRating > 0 ? $"{book.AverageRating} / 5" : "-",
                    status
                );
                dgvBooks.Rows[rowIdx].Tag = book;
            }

            // Grid view güncelle
            flpGridBooks.Controls.Clear();
            foreach (var item in list)
            {
                var pnl = new Panel { Width = 160, Height = 220, BackColor = Color.FromArgb(22, 27, 34), Margin = new Padding(15) };
                
                Control headerElement;
                if (!string.IsNullOrEmpty(item.CoverImagePath) && System.IO.File.Exists(item.CoverImagePath))
                {
                    headerElement = new PictureBox { ImageLocation = item.CoverImagePath, SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Top, Height = 130 };
                }
                else
                {
                    headerElement = new Label { Text = "📘", Font = new Font("Segoe UI", 48), ForeColor = Color.FromArgb(99,102,241), Dock = DockStyle.Top, Height = 130, TextAlign = ContentAlignment.MiddleCenter };
                }
                
                var lblTitle2 = new Label { Text = item.Title, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10, 5, 10, 0) };
                var lblAuthor = new Label { Text = item.Author?.FullName ?? "Bilinmiyor", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(139, 148, 158), Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(10, 0, 10, 0) };
                
                pnl.Controls.Add(lblAuthor);
                pnl.Controls.Add(lblTitle2);
                pnl.Controls.Add(headerElement);
                
                var bookRef = item;
                EventHandler onClick = (s, e) => ShowRightSidebar(bookRef);
                pnl.Click += onClick; headerElement.Click += onClick; lblTitle2.Click += onClick; lblAuthor.Click += onClick;

                EventHandler onDoubleClick = (s, e) => {
                    if (item.HasDigitalCopy) {
                        OpenBookDigital(item);
                    }
                };
                pnl.DoubleClick += onDoubleClick; headerElement.DoubleClick += onDoubleClick; lblTitle2.DoubleClick += onDoubleClick; lblAuthor.DoubleClick += onDoubleClick;

                MouseEventHandler onMouseDown = (s, e) => {
                    if (e.Button == MouseButtons.Right) {
                        ShowRightSidebar(bookRef);
                    }
                };
                pnl.MouseDown += onMouseDown; headerElement.MouseDown += onMouseDown; lblTitle2.MouseDown += onMouseDown; lblAuthor.MouseDown += onMouseDown;

                pnl.ContextMenuStrip = bookContextMenu;
                headerElement.ContextMenuStrip = bookContextMenu;
                lblTitle2.ContextMenuStrip = bookContextMenu;
                lblAuthor.ContextMenuStrip = bookContextMenu;

                flpGridBooks.Controls.Add(pnl);
            }

            lblPageTitle.Text = list.Count == _allBooks.Count ? "Tüm Kitaplar" : $"Filtrelendi ({list.Count} kitap)";
        }
    }
}
