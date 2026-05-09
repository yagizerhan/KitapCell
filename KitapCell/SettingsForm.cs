using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using KitapCell.Models;
using KitapCell.Core;
using KitapCell.Repositories;
using KitapCell.Data;

namespace KitapCell
{
    public class SettingsForm : Form
    {
        private Panel _pnlSidebar;
        private Panel _pnlContent;

        private Button _btnMenuGenel;
        private Button _btnMenuWebSunucu;
        private Button _btnMenuYetkiler;
        private Button _btnMenuVeritabani;
        private Button _btnMenuHakkinda;
        private Button _btnMenuImport;

        public SettingsForm()
        {
            InitializeUI();
            this.Load += (s, e) => ShowTab_Genel();
            ThemeHelper.Apply(this);
        }

        private void InitializeUI()
        {
            this.Text = "Sistem Ayarları";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(900, 600);

            // --- Sol Sidebar ---
            _pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(10)
            };

            var lblTitle = new Label
            {
                Text = "⚙️ Ayarlar",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(10, 20),
                Width = 240,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(99, 102, 241)
            };

            int menuStartY = 90;
            _btnMenuGenel       = CreateMenuButton("⚙️ Genel Ayarlar",         menuStartY);
            _btnMenuWebSunucu   = CreateMenuButton("🌐 Web Sunucu",            menuStartY + 50);
            _btnMenuYetkiler    = CreateMenuButton("👥 Yetkiler ve Roller",    menuStartY + 100);
            _btnMenuVeritabani  = CreateMenuButton("💾 Veritabanı ve Bakım",  menuStartY + 150);
            _btnMenuHakkinda    = CreateMenuButton("ℹ️ Hakkında",              menuStartY + 200);
            _btnMenuImport      = CreateMenuButton("📂 Toplu İçe Aktar",       menuStartY + 250);

            _btnMenuGenel.Click      += (s, e) => ShowTab_Genel();
            _btnMenuWebSunucu.Click  += (s, e) => ShowTab_WebSunucu();
            _btnMenuYetkiler.Click   += (s, e) => ShowTab_Yetkiler();
            _btnMenuVeritabani.Click += (s, e) => ShowTab_Veritabani();
            _btnMenuHakkinda.Click   += (s, e) => ShowTab_Hakkinda();
            _btnMenuImport.Click     += (s, e) => ShowTab_TopluImport();

            _pnlSidebar.Controls.AddRange(new Control[] { 
                lblTitle, _btnMenuGenel, _btnMenuWebSunucu, _btnMenuYetkiler,
                _btnMenuVeritabani, _btnMenuHakkinda, _btnMenuImport
            });

            // --- Sağ İçerik Alanı ---
            _pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(13, 17, 23),
                Padding = new Padding(30)
            };

            this.Controls.Add(_pnlContent);
            this.Controls.Add(_pnlSidebar);
        }

        private Button CreateMenuButton(string text, int yPos)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(10, yPos),
                Size = new Size(240, 45),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(201, 209, 217),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void SetActiveButton(Button activeBtn)
        {
            Button[] btns = { _btnMenuGenel, _btnMenuWebSunucu, _btnMenuYetkiler, _btnMenuVeritabani, _btnMenuHakkinda, _btnMenuImport };
            foreach (var b in btns)
            {
                b.BackColor = Color.Transparent;
                b.ForeColor = Color.FromArgb(201, 209, 217);
                b.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            }
            activeBtn.BackColor = Color.FromArgb(35, 40, 58);
            activeBtn.ForeColor = Color.White;
            activeBtn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        }

        private void ClearContent(string title)
        {
            _pnlContent.Controls.Clear();
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                ForeColor = Color.White
            };
            _pnlContent.Controls.Add(lblTitle);
        }

        private void ShowTab_Genel()
        {
            SetActiveButton(_btnMenuGenel);
            ClearContent("⚙️ Genel Ayarlar");

            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };

            var lblLoan = new Label { Text = "Varsayılan Ödünç Süresi (Gün):", AutoSize = true, Font = new Font("Segoe UI", 11F), Location = new Point(0, 20) };
            var numLoan = new NumericUpDown { 
                Minimum = 1, Maximum = 365, 
                Value = SettingsManager.Config.DefaultLoanDays, 
                Location = new Point(250, 18), 
                Width = 100,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(22, 27, 34),
                ForeColor = Color.White
            };

            var lblPdf = new Label { Text = "PDF Açma Tercihi:", AutoSize = true, Font = new Font("Segoe UI", 11F), Location = new Point(0, 80) };
            var pnlRadio = new Panel { Location = new Point(0, 110), Size = new Size(400, 35) };
            
            var rbInApp = new RadioButton { Text = "Dahili Okuyucu (Uygulama İçi)", AutoSize = true, ForeColor = Color.White, Location = new Point(0, 5) };
            var rbSystem = new RadioButton { Text = "Sistem Varsayılan Uygulaması", AutoSize = true, ForeColor = Color.White, Location = new Point(230, 5) };
            
            pnlRadio.Controls.AddRange(new Control[] { rbInApp, rbSystem });
            
            if (SettingsManager.Config.PdfOpenMode == PdfOpenMode.System)
                rbSystem.Checked = true;
            else
                rbInApp.Checked = true;

            var btnSave = new Button {
                Text = "Kaydet",
                Location = new Point(0, 170),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => {
                SettingsManager.Config.DefaultLoanDays = (int)numLoan.Value;
                SettingsManager.Config.PdfOpenMode = rbSystem.Checked ? PdfOpenMode.System : PdfOpenMode.InApp;
                SettingsManager.Save();
                MessageBox.Show("Ayarlar başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pnlMain.Controls.AddRange(new Control[] { lblLoan, numLoan, lblPdf, pnlRadio, btnSave });
            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }

        private async void ShowTab_Yetkiler()
        {
            SetActiveButton(_btnMenuYetkiler);
            ClearContent("👥 Yetkiler ve Roller");

            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };
            
            var lblSelect = new Label { Text = "Kullanıcı Seçin:", AutoSize = true, Font = new Font("Segoe UI", 11F), Location = new Point(0, 0) };
            var cmbUsers = new ComboBox { 
                Location = new Point(0, 30), 
                Width = 400,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(22, 27, 34),
                ForeColor = Color.White
            };

            var chkAdd = new CheckBox { Text = "Kitap Ekleme Yetkisi (Can Add Book)", AutoSize = true, Location = new Point(0, 80), Font = new Font("Segoe UI", 11F), Visible = false };
            var chkEdit = new CheckBox { Text = "Kitap Düzenleme Yetkisi (Can Edit Book)", AutoSize = true, Location = new Point(0, 115), Font = new Font("Segoe UI", 11F), Visible = false };
            var chkDelete = new CheckBox { Text = "Kitap Silme Yetkisi (Can Delete Book)", AutoSize = true, Location = new Point(0, 150), Font = new Font("Segoe UI", 11F), Visible = false };

            var btnSave = new Button {
                Text = "Yetkileri Güncelle",
                Location = new Point(0, 210),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnSave.FlatAppearance.BorderSize = 0;

            pnlMain.Controls.AddRange(new Control[] { lblSelect, cmbUsers, chkAdd, chkEdit, chkDelete, btnSave });
            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            // Kullanıcı listesini yükle
            List<User> nonAdmins;
            using (var db = new LibraryDbContext())
            {
                var userRepo = new UserRepository(db);
                var users = await userRepo.GetAllAsync();
                nonAdmins = users.Where(u => u.Role != UserRole.Admin).ToList();
            }

            cmbUsers.DataSource = nonAdmins;
            cmbUsers.DisplayMember = "Email";
            cmbUsers.ValueMember = "Id";

            User selectedUser = null;

            cmbUsers.SelectedIndexChanged += async (s, e) => {
                if (cmbUsers.SelectedItem is User u) {
                    try {
                        // Her seçimde yeni context aç — disposed context hatası olmaz
                        using var db2 = new LibraryDbContext();
                        var userRepo2 = new UserRepository(db2);
                        selectedUser = await userRepo2.GetByIdAsync(u.Id);
                        if (selectedUser != null) {
                            chkAdd.Checked = selectedUser.CanAddBook;
                            chkEdit.Checked = selectedUser.CanEditBook;
                            chkDelete.Checked = selectedUser.CanDeleteBook;
                            chkAdd.Visible = chkEdit.Visible = chkDelete.Visible = btnSave.Visible = true;
                        }
                    } catch (Exception ex) {
                        MessageBox.Show("Kullanıcı yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            // İlk tetikleme
            if (cmbUsers.Items.Count > 0) cmbUsers.SelectedIndex = 0;

            btnSave.Click += async (s, e) => {
                if (selectedUser == null) return;
                selectedUser.CanAddBook = chkAdd.Checked;
                selectedUser.CanEditBook = chkEdit.Checked;
                selectedUser.CanDeleteBook = chkDelete.Checked;

                try {
                    using var context = new LibraryDbContext();
                    context.Users.Update(selectedUser);
                    await context.SaveChangesAsync();
                    MessageBox.Show($"{selectedUser.FirstName} yetkileri güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch (Exception ex) {
                    MessageBox.Show("Yetki kaydedilirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void ShowTab_Veritabani()
        {
            SetActiveButton(_btnMenuVeritabani);
            ClearContent("💾 Veritabanı ve Bakım");

            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };

            var lblDB = new Label { Text = "Bu bölümden sistem veritabanının yedeğini alabilir, önceden alınmış bir yedeği sisteme yükleyebilir veya tüm veritabanını fabrika ayarlarına sıfırlayabilirsiniz.", 
                                    Width = 600, Height = 60, Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(139, 148, 158) };
            
            var btnBackup = new Button { Text = "🔽 Yedek Al (Backup)", Location = new Point(0, 80), Size = new Size(200, 45), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnBackup.FlatAppearance.BorderSize = 0;
            
            var btnRestore = new Button { Text = "🔼 Geri Yükle (Restore)", Location = new Point(220, 80), Size = new Size(200, 45), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnRestore.FlatAppearance.BorderSize = 0;

            var btnReset = new Button { Text = "🧨 Sistemi Sıfırla (Factory Reset)", Location = new Point(0, 150), Size = new Size(300, 45), BackColor = Color.Transparent, ForeColor = Color.FromArgb(239, 68, 68), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(239, 68, 68);

            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KitapCell");
            string dbPath = Path.Combine(appDataFolder, "library.db");

            btnBackup.Click += (s, e) => {
                using var sfd = new SaveFileDialog();
                sfd.Filter = "Veritabanı Dosyaları (*.db)|*.db";
                sfd.FileName = $"library_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                if(sfd.ShowDialog() == DialogResult.OK) {
                    try {
                        File.Copy(dbPath, sfd.FileName, true);
                        MessageBox.Show("Veritabanı yedeği başarıyla alındı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch(Exception ex) {
                        MessageBox.Show("Yedek alınırken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            btnRestore.Click += (s, e) => {
                var ask = MessageBox.Show("Geri yükleme işlemi, mevcut tüm verilerinizin üzerine yazılır. \n\nEmin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ask != DialogResult.Yes) return;

                using var ofd = new OpenFileDialog();
                ofd.Filter = "Veritabanı Dosyaları (*.db)|*.db";
                if(ofd.ShowDialog() == DialogResult.OK) {
                    try {
                        File.Copy(ofd.FileName, dbPath, true);
                        MessageBox.Show("Veritabanı geri yüklendi. Değişikliklerin etkili olması için uygulama kapatılacaktır. Lütfen yeniden başlatın.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    } catch(Exception ex) {
                        MessageBox.Show("Geri yükleme başarısız. Uygulama veritabanını kullanıyor olabilir:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            btnReset.Click += (s, e) => {
                var ask1 = MessageBox.Show("Sistemi sıfırlamak (Factory Reset), kayıtlı tüm kitapları, kullanıcıları, ödünç kayıtlarını ve favorileri KALICI olarak siler.\n\nEmin misiniz?", "Kritik Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ask1 != DialogResult.Yes) return;

                var ask2 = MessageBox.Show("Bu işlemin GERİ DÖNÜŞÜ YOKTUR. Tüm veriler silinecek. GERÇEKTEN sıfırlamak istiyor musunuz?", "Son Kararınız Mı?", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (ask2 == DialogResult.Yes) {
                    try {
                        using var context = new LibraryDbContext();
                        context.Database.EnsureDeleted();
                        MessageBox.Show("Sistem başarıyla sıfırlandı. Uygulama şimdi kapatılacaktır. Yeniden başlattığınızda temiz bir kurulumla başlayacaktır.", "Reset Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    } catch(Exception ex) {
                        MessageBox.Show("Sıfırlama başarısız:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            pnlMain.Controls.AddRange(new Control[] { lblDB, btnBackup, btnRestore, btnReset });
            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }

        private async void ShowTab_Hakkinda()
        {
            SetActiveButton(_btnMenuHakkinda);
            ClearContent("ℹ️ Sistem Bilgisi");

            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };

            int totalBooks = 0, totalUsers = 0, totalLoans = 0;
            try {
                using var db = new LibraryDbContext();
                totalBooks = db.Books.Count();
                totalUsers = db.Users.Count();
                totalLoans = db.BookLoans.Count();
            } catch { }

            string infoHtml = $@"
KitapCell - Modern Kütüphane Yönetim Sistemi
Versiyon: v1.4 (Bahar Dönemi Ödevi)

Geliştirici Sürümü
C# ve Windows Forms teknolojileri ile oluşturulmuştur.
Modern arayüz, gelişmiş karanlık tema ve pürüzsüz animasyonlarıyla 
kullanıcı dostu kütüphane deneyimi.

-------------------------
📊 SİSTEM İSTATİSTİKLERİ
-------------------------
Toplam Kitap Kaydı: {totalBooks}
Sisteme Kayıtlı Üye: {totalUsers}
Gerçekleşen Ödünç İşlemleri: {totalLoans}
Yürütülebilir Ortam: .NET 9.0
Veritabanı Formatı: SQLite (Entity Framework Core)
".Trim();

            var txtInfo = new TextBox {
                Multiline = true,
                ReadOnly = true,
                Text = infoHtml,
                Dock = DockStyle.Top,
                Height = 300,
                Font = new Font("Consolas", 11F),
                BackColor = Color.FromArgb(22, 27, 34),
                ForeColor = Color.FromArgb(201, 209, 217),
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10)
            };

            pnlMain.Controls.Add(txtInfo);
            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }

        // ── Web Sunucu Sekmesi ────────────────────────────────────────────────────

        private System.Windows.Forms.Timer? _logRefreshTimer;

        private void ShowTab_WebSunucu()
        {
            SetActiveButton(_btnMenuWebSunucu);
            ClearContent("🌐 Web Sunucu Ayarları");

            var pnlMain = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            int y = 10;

            // ── Durum satırı ──────────────────────────────────────────────────────
            var pnlStatus = new Panel
            {
                Location = new Point(0, y), Height = 56, BackColor = Color.FromArgb(22, 27, 34)
            };
            pnlStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblStatusTitle = new Label
            {
                Text = "Sunucu Durumu", Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(139, 148, 158),
                Location = new Point(16, 6), AutoSize = true
            };
            var lblStatusVal = new Label
            {
                Name = "lblStatusVal",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                Location = new Point(16, 24), AutoSize = true
            };
            pnlStatus.Controls.AddRange(new Control[] { lblStatusTitle, lblStatusVal });
            pnlMain.Controls.Add(pnlStatus);
            y += 66;

            // ── URL gösterimi ─────────────────────────────────────────────────────
            var pnlUrl = new Panel
            {
                Location = new Point(0, y), Height = 52, BackColor = Color.FromArgb(18, 22, 29)
            };
            pnlUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblUrl = new Label
            {
                Name = "lblUrl",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 102, 241),
                Location = new Point(16, 16), AutoSize = true
            };
            var btnCopyUrl = new Button
            {
                Text = "📋 Kopyala",
                Location = new Point(450, 14), Size = new Size(110, 28),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(35, 40, 58),
                ForeColor = Color.White, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand
            };
            btnCopyUrl.FlatAppearance.BorderSize = 0;
            btnCopyUrl.Click += (s, e) =>
            {
                if (Web.WebServer.IsRunning)
                    Clipboard.SetText(Web.WebServer.GetNetworkUrl());
            };
            pnlUrl.Controls.AddRange(new Control[] { lblUrl, btnCopyUrl });
            pnlMain.Controls.Add(pnlUrl);
            y += 62;

            // ── Port Ayarı ────────────────────────────────────────────────────────
            var pnlPort = new Panel
            {
                Location = new Point(0, y), Height = 54, BackColor = Color.Transparent
            };
            pnlPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblPort = new Label
            {
                Text = "Port Numarası:", Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(201, 209, 217),
                Location = new Point(0, 16), AutoSize = true
            };
            var txtPort = new System.Windows.Forms.TextBox
            {
                Text = SettingsManager.Config.WebServerPort.ToString(),
                Location = new Point(140, 12), Size = new Size(90, 28),
                BackColor = Color.FromArgb(33, 38, 45),
                ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            var btnSavePort = new Button
            {
                Text = "💾 Kaydet",
                Location = new Point(240, 12), Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSavePort.FlatAppearance.BorderSize = 0;
            btnSavePort.Click += (s, e) =>
            {
                if (int.TryParse(txtPort.Text, out int port) && port > 1000 && port < 65535)
                {
                    SettingsManager.Config.WebServerPort = port;
                    SettingsManager.Save();
                    MessageBox.Show($"Port {port} kaydedildi. Değişikliğin geçerli olması için sunucuyu yeniden başlatın.",
                        "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Geçerli bir port numarası girin (1001-65534).",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            pnlPort.Controls.AddRange(new Control[] { lblPort, txtPort, btnSavePort });
            pnlMain.Controls.Add(pnlPort);
            y += 62;

            // ── Erişim Modu ────────────────────────────────────────────────────────
            var pnlAccess = new Panel
            {
                Location = new Point(0, y), Height = 140,
                BackColor = Color.FromArgb(18, 22, 29)
            };
            pnlAccess.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblAccessTitle = new Label
            {
                Text = "🔐 Erişim Modu",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(201, 209, 217),
                Location = new Point(16, 10), AutoSize = true
            };

            var rbLoginRequired = new RadioButton
            {
                Text = "Login Gerekli  (Önerilen)",
                Location = new Point(16, 36),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White
            };
            var lblLoginDesc = new Label
            {
                Text = "Tüm içeriklere erişim için kullanıcı girişi zorunludur.",
                Location = new Point(36, 58),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(139, 148, 158)
            };

            var rbGuestOpen = new RadioButton
            {
                Text = "Misafir Erişimi Açık",
                Location = new Point(16, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 102, 241)
            };
            var lblGuestDesc = new Label
            {
                Text = "Kayıtsız ziyaretçiler kitap listesine ve PDF/EPUB içeriklere erişebilir.",
                Location = new Point(36, 102),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(139, 148, 158)
            };

            // Mevcut ayarı yansıt
            if (SettingsManager.Config.RequireLoginForWebServer)
                rbLoginRequired.Checked = true;
            else
                rbGuestOpen.Checked = true;

            var btnSaveAccess = new Button
            {
                Text = "💾 Erişim Modunu Kaydet",
                Location = new Point(16, 128),
                Size = new Size(220, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveAccess.FlatAppearance.BorderSize = 0;
            btnSaveAccess.Click += (s, e) =>
            {
                SettingsManager.Config.RequireLoginForWebServer = rbLoginRequired.Checked;
                SettingsManager.Save();
                string modMsg = rbLoginRequired.Checked
                    ? "Login Gerekli modu aktif.\nTüm içerikler için giriş zorunlu."
                    : "Misafir Erişimi aktif.\nKitap ve PDF/EPUB içerikleri giriş olmadan görüntülenebilir.";
                MessageBox.Show(modMsg + "\n\nDeğişiklik hemen geçerli olur.", "Erişim Modu Kaydedildi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pnlAccess.Height = 172; // içeriklere göre boyutu genişlet
            pnlAccess.Controls.AddRange(new Control[] {
                lblAccessTitle, rbLoginRequired, lblLoginDesc,
                rbGuestOpen, lblGuestDesc, btnSaveAccess
            });
            pnlMain.Controls.Add(pnlAccess);
            y += 182;

            // ── Başlat / Durdur ────────────────────────────────────────────────────
            var btnStartStop = new Button
            {
                Name = "btnStartStop",
                Location = new Point(0, y), Size = new Size(200, 42),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White, Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnStartStop.FlatAppearance.BorderSize = 0;
            pnlMain.Controls.Add(btnStartStop);
            y += 56;

            // ── Log Paneli ────────────────────────────────────────────────────────
            var lblLogTitle = new Label
            {
                Text = "📋 Sunucu Günlüğü", Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White, Location = new Point(0, y), AutoSize = true
            };
            pnlMain.Controls.Add(lblLogTitle);
            y += 30;

            var txtLog = new RichTextBox
            {
                Location = new Point(0, y),
                BackColor = Color.FromArgb(13, 17, 23),
                ForeColor = Color.FromArgb(139, 148, 158),
                Font = new Font("Consolas", 9.5F),
                ReadOnly = true, BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Size = new Size(pnlMain.Width - 10, 220);
            pnlMain.Controls.Add(txtLog);
            y += 228;

            var btnClearLog = new Button
            {
                Text = "🗑️ Günlüğü Temizle", Location = new Point(0, y),
                Size = new Size(180, 32),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(35, 40, 58),
                ForeColor = Color.FromArgb(201, 209, 217),
                Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand
            };
            btnClearLog.FlatAppearance.BorderSize = 0;
            btnClearLog.Click += (s, e) =>
            {
                Web.WebServer.Logs.Clear();
                txtLog.Clear();
            };
            pnlMain.Controls.Add(btnClearLog);

            // ── Durumu Güncelle (yardımcı) ─────────────────────────────────────────
            void RefreshStatus()
            {
                bool running = Web.WebServer.IsRunning;
                lblStatusVal.Text      = running ? "🟢 Çalışıyor" : "🔴 Durduruldu";
                lblStatusVal.ForeColor = running ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                lblUrl.Text            = running ? Web.WebServer.GetNetworkUrl() : "—";
                btnStartStop.Text      = running ? "⛔  Durdur" : "▶️  Başlat";
                btnStartStop.BackColor = running ? Color.FromArgb(239, 68, 68) : Color.FromArgb(34, 197, 94);

                // Logları güncelle
                if (txtLog != null && !txtLog.IsDisposed)
                {
                    txtLog.Text = string.Join(Environment.NewLine, Web.WebServer.Logs.ToArray());
                    txtLog.SelectionStart = txtLog.TextLength;
                    txtLog.ScrollToCaret();
                }
            }

            RefreshStatus();

            // Log evt — handler değişkende saklanır; sekme değişince unsubscribe edilir
            Action<string> logHandler = null!;
            logHandler = entry =>
            {
                // Handle oluşturulmadan veya dispose sonrası Invoke'u engelle
                if (txtLog.IsDisposed || !txtLog.IsHandleCreated) return;
                try
                {
                    txtLog.BeginInvoke((Action)(() =>
                    {
                        if (txtLog.IsDisposed) return;
                        txtLog.AppendText(entry + Environment.NewLine);
                        txtLog.SelectionStart = txtLog.TextLength;
                        txtLog.ScrollToCaret();
                        RefreshStatus();
                    }));
                }
                catch { /* sekme değişimi sırasındaki geçici durum — yoksay */ }
            };
            Web.WebServer.LogAdded += logHandler;

            // Sekme değişince (pnlMain dispose) handler'ı kaldır → event sızıntısı önlenir
            pnlMain.Disposed += (s, e) => Web.WebServer.LogAdded -= logHandler;

            // Başlat/Durdur butonu
            btnStartStop.Click += async (s, e) =>
            {
                btnStartStop.Enabled = false;
                if (Web.WebServer.IsRunning)
                    await Web.WebServer.StopAsync();
                else
                {
                    int port = SettingsManager.Config.WebServerPort;
                    await Web.WebServer.StartAsync(port);
                }
                RefreshStatus();
                btnStartStop.Enabled = true;
            };

            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }

        // ── Toplu İçe Aktar Sekmesi ───────────────────────────────────────────────

        private void ShowTab_TopluImport()
        {
            SetActiveButton(_btnMenuImport);
            ClearContent("📂 Toplu İçe Aktar");

            var pnlMain = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(0, 10, 0, 0) };

            int y = 0;

            // Açıklama
            var lblDesc = new Label
            {
                Text = "Bir klasör seçerek içindeki tüm PDF ve EPUB dosyalarını kütüphaneye toplu olarak ekleyebilirsiniz.\nDosya adları kitap başlığı olarak kullanılır. Yazar \"Bilinmiyor\" olarak atanır.",
                Location = new Point(0, y), AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(139, 148, 158)
            };
            pnlMain.Controls.Add(lblDesc);
            y += 60;

            // Klasör seç butonu + seçilen yol
            var btnSelectFolder = new Button
            {
                Text = "📁 Klasör Seç",
                Location = new Point(0, y), Size = new Size(160, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSelectFolder.FlatAppearance.BorderSize = 0;

            var lblFolderPath = new Label
            {
                Text = "Henüz klasör seçilmedi.",
                Location = new Point(170, y + 10), AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(139, 148, 158)
            };

            pnlMain.Controls.AddRange(new Control[] { btnSelectFolder, lblFolderPath });
            y += 54;

            // Kategori alanı kaldırıldı, sabit kategori (1) kullanılacak


            // Dosya listesi
            var lblFileList = new Label
            {
                Text = "Bulunan Dosyalar:", AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, y)
            };
            pnlMain.Controls.Add(lblFileList);
            y += 26;

            var listView = new ListView
            {
                Location = new Point(0, y),
                Size = new Size(620, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(22, 27, 34),
                ForeColor = Color.FromArgb(201, 209, 217),
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };
            listView.Columns.Add("Dosya Adı", 350);
            listView.Columns.Add("Tür", 60);
            listView.Columns.Add("Boyut", 90);
            listView.Columns.Add("Durum", 110);
            listView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlMain.Controls.Add(listView);
            y += 210;

            // Progress bar
            var progress = new ProgressBar
            {
                Location = new Point(0, y),
                Size = new Size(620, 24),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };
            progress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlMain.Controls.Add(progress);
            y += 30;

            // Durum etiketi
            var lblStatus = new Label
            {
                Text = "",
                Location = new Point(0, y), AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94)
            };
            pnlMain.Controls.Add(lblStatus);
            y += 30;

            // İçe Aktar butonu
            var btnImport = new Button
            {
                Text = "🚀 Tümünü İçe Aktar",
                Location = new Point(0, y), Size = new Size(220, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnImport.FlatAppearance.BorderSize = 0;
            pnlMain.Controls.Add(btnImport);

            // Bulunan dosya yollarını tutacak liste
            var foundFiles = new System.Collections.Generic.List<string>();

            // ── Klasör Seç ─────────────────────────────────────────────────────────
            btnSelectFolder.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog();
                fbd.Description = "PDF ve EPUB dosyalarının bulunduğu klasörü seçin";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() != DialogResult.OK) return;

                lblFolderPath.Text = fbd.SelectedPath;
                lblFolderPath.ForeColor = Color.FromArgb(99, 102, 241);
                listView.Items.Clear();
                foundFiles.Clear();
                lblStatus.Text = "";

                // Seçilen klasörü tara (sadece direkt içerik, alt klasör yok)
                var extensions = new[] { ".pdf", ".epub" };
                var files = Directory.GetFiles(fbd.SelectedPath)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderBy(f => f)
                    .ToList();

                if (files.Count == 0)
                {
                    lblStatus.Text = "Bu klasörde PDF veya EPUB dosyası bulunamadı.";
                    lblStatus.ForeColor = Color.FromArgb(245, 158, 11);
                    btnImport.Enabled = false;
                    return;
                }

                foreach (var file in files)
                {
                    foundFiles.Add(file);
                    var fi = new FileInfo(file);
                    var ext = Path.GetExtension(file).ToUpperInvariant().TrimStart('.');
                    var sizeMB = (fi.Length / 1024.0 / 1024.0).ToString("F1") + " MB";

                    var item = new ListViewItem(Path.GetFileNameWithoutExtension(file));
                    item.SubItems.Add(ext);
                    item.SubItems.Add(sizeMB);
                    item.SubItems.Add("Bekliyor");
                    listView.Items.Add(item);
                }

                lblStatus.Text = $"{files.Count} dosya bulundu.";
                lblStatus.ForeColor = Color.FromArgb(34, 197, 94);
                btnImport.Enabled = true;
            };

            // ── İçe Aktar ──────────────────────────────────────────────────────────
            btnImport.Click += async (s, e) =>
            {
                if (foundFiles.Count == 0) return;

                btnImport.Enabled = false;
                btnSelectFolder.Enabled = false;
                progress.Visible = true;
                progress.Minimum = 0;
                progress.Maximum = foundFiles.Count;
                progress.Value = 0;

                int success = 0, skipped = 0, errors = 0;

                string assetsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                string coversFolder = Path.Combine(assetsFolder, "Covers");
                string pdfsFolder = Path.Combine(assetsFolder, "Pdfs");
                if (!Directory.Exists(coversFolder)) Directory.CreateDirectory(coversFolder);
                if (!Directory.Exists(pdfsFolder)) Directory.CreateDirectory(pdfsFolder);

                int categoryId = 1; // Sabit kategori (Genel vb.)


                for (int i = 0; i < foundFiles.Count; i++)
                {
                    var filePath = foundFiles[i];
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var lvItem = listView.Items[i];

                    try
                    {
                        using var db = new LibraryDbContext();

                        // Duplicate kontrolü: aynı başlıkta kitap var mı?
                        bool exists = db.Books.Any(b => b.Title == fileName);
                        if (exists)
                        {
                            lvItem.SubItems[3].Text = "⏭️ Zaten mevcut";
                            lvItem.ForeColor = Color.FromArgb(245, 158, 11);
                            skipped++;
                            progress.Value = i + 1;
                            continue;
                        }

                        // Yazar: "Bilinmiyor"
                        var author = db.Authors.FirstOrDefault(a => a.FullName == "Bilinmiyor");
                        if (author == null)
                        {
                            author = new Author { FullName = "Bilinmiyor", Biography = "Toplu içe aktarma ile eklendi." };
                            db.Authors.Add(author);
                            await db.SaveChangesAsync();
                        }

                        // Dosyayı Assets/Pdfs/ altına kopyala
                        string ext = Path.GetExtension(filePath);
                        string newFileName = Guid.NewGuid().ToString() + ext;
                        string savedPdfPath = Path.Combine(pdfsFolder, newFileName);
                        File.Copy(filePath, savedPdfPath, true);

                        // Otomatik kapak çıkarımı (sadece PDF)
                        string savedCoverPath = null;
                        if (ext.ToLowerInvariant() == ".pdf")
                        {
                            try
                            {
                                using var pdfDoc = PdfiumViewer.PdfDocument.Load(savedPdfPath);
                                if (pdfDoc.PageCount > 0)
                                {
                                    using var img = pdfDoc.Render(0, 150, 150, true);
                                    string cvrFileName = Guid.NewGuid().ToString() + "_auto.png";
                                    savedCoverPath = Path.Combine(coversFolder, cvrFileName);
                                    img.Save(savedCoverPath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            catch { /* Kapak çıkarılamazsa devam et */ }
                        }

                        // Kitap oluştur
                        var book = new Book
                        {
                            Title = fileName,
                            AuthorId = author.Id,
                            CategoryId = categoryId,
                            ISBN = "BULK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                            TotalCopies = 1,
                            AvailableCopies = 1,
                            CoverImagePath = savedCoverPath,
                            PdfFilePath = savedPdfPath,
                            HasDigitalCopy = true,
                            AddedDate = DateTime.Now
                        };
                        db.Books.Add(book);
                        await db.SaveChangesAsync();

                        lvItem.SubItems[3].Text = "✅ Eklendi";
                        lvItem.ForeColor = Color.FromArgb(34, 197, 94);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        lvItem.SubItems[3].Text = $"❌ Hata";
                        lvItem.ForeColor = Color.FromArgb(239, 68, 68);
                        errors++;
                        System.Diagnostics.Debug.WriteLine($"Import error [{fileName}]: {ex.Message}");
                    }

                    progress.Value = i + 1;
                    // UI thread'i bloklamadan güncellemeyi göster
                    Application.DoEvents();
                }

                lblStatus.Text = $"Tamamlandı! ✅ {success} eklendi  |  ⏭️ {skipped} atlandı  |  ❌ {errors} hata";
                lblStatus.ForeColor = errors > 0 ? Color.FromArgb(245, 158, 11) : Color.FromArgb(34, 197, 94);

                btnSelectFolder.Enabled = true;
                btnImport.Enabled = true;

                MessageBox.Show(
                    $"Toplu içe aktarma tamamlandı!\n\n" +
                    $"✅ Başarıyla eklenen: {success}\n" +
                    $"⏭️ Zaten mevcut (atlanan): {skipped}\n" +
                    $"❌ Hatalı: {errors}",
                    "İçe Aktarma Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _pnlContent.Controls.Add(pnlMain);
            pnlMain.BringToFront();
        }
    }
}
