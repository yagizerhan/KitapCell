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
    public class ProfileForm : Form
    {
        private User _user;
        private PictureBox _picProfile;
        private Label _lblFullName;
        private Label _lblEmail;

        private Panel _pnlSidebar;
        private Panel _pnlContent;

        private Button _btnMenuInfo;
        private Button _btnMenuHistory;
        private Button _btnMenuFavs;
        private Button _btnMenuRatings;
        private Button _btnMenuLogout;

        public bool LogoutRequested { get; private set; } = false;

        public ProfileForm()
        {
            _user = GlobalSession.CurrentUser ?? throw new Exception("Oturum açık değil!");
            InitializeUI();
            
            // Verileri yükledikten sonra Bilgilerim sekmesi açılsın
            this.Load += async (s, e) => {
                await ReloadUserDataAsync();
                ShowTab_Info();
            };
        }

        private void InitializeUI()
        {
            this.Text = "Kullanıcı Profili";
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

            _picProfile = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(70, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(18, 22, 29)
            };

            _lblFullName = new Label
            {
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                Location = new Point(10, 160),
                Width = 240,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(99, 102, 241)
            };

            _lblEmail = new Label
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(10, 190),
                Width = 240,
                Height = 25,
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(139, 148, 158)
            };

            var btnChangePhoto = new Button
            {
                Text = "📷 Fotoğraf",
                Location = new Point(70, 220),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(35, 40, 58),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnChangePhoto.FlatAppearance.BorderSize = 0;
            btnChangePhoto.Click += BtnChangePhoto_Click;

            int menuStartY = 280;
            _btnMenuInfo = CreateMenuButton("👤 Bilgilerim", menuStartY);
            _btnMenuHistory = CreateMenuButton("📚 Okuma & Ödünç", menuStartY + 50);
            _btnMenuFavs = CreateMenuButton("❤️ Favorilerim", menuStartY + 100);
            _btnMenuRatings = CreateMenuButton("⭐ Yorumlarım", menuStartY + 150);
            
            _btnMenuLogout = CreateMenuButton("🚪 Çıkış Yap", menuStartY + 230);
            _btnMenuLogout.ForeColor = Color.FromArgb(239, 68, 68);
            _btnMenuLogout.Click += (s, e) => {
                var res = MessageBox.Show("Hesabınızdan çıkış yapmak istediğinize emin misiniz?", "Çıkış Yap", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    LogoutRequested = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            _btnMenuInfo.Click += (s, e) => ShowTab_Info();
            _btnMenuHistory.Click += (s, e) => ShowTab_History();
            _btnMenuFavs.Click += (s, e) => ShowTab_Favs();
            _btnMenuRatings.Click += (s, e) => ShowTab_Ratings();

            _pnlSidebar.Controls.AddRange(new Control[] { 
                _picProfile, _lblFullName, _lblEmail, btnChangePhoto,
                _btnMenuInfo, _btnMenuHistory, _btnMenuFavs, _btnMenuRatings, _btnMenuLogout
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
            ThemeHelper.Apply(this);
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
            Button[] btns = { _btnMenuInfo, _btnMenuHistory, _btnMenuFavs, _btnMenuRatings };
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

        private async Task ReloadUserDataAsync()
        {
            using var db = new LibraryDbContext();
            var userRepo = new UserRepository(db);
            var freshUser = await userRepo.GetByIdWithDetailsAsync(_user.Id);
            if (freshUser != null) _user = freshUser;

            _lblFullName.Text = $"{_user.FirstName} {_user.LastName}";
            _lblEmail.Text = _user.Email;

            if (!string.IsNullOrEmpty(_user.ProfileImagePath) && File.Exists(_user.ProfileImagePath))
            {
                try { _picProfile.Image = Image.FromFile(_user.ProfileImagePath); } catch { }
            }
        }

        // --- Sekmeler (Tabs) ---

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

        private void ShowTab_Info()
        {
            SetActiveButton(_btnMenuInfo);
            ClearContent("👤  Kişisel Bilgiler");

            var pnlInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 5,
                Height = 350,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Padding = new Padding(0, 20, 0, 0)
            };
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            string roleText = _user.Role == UserRole.Admin ? "Sistem Yöneticisi" : "Üye";

            void AddRow(string label, string val)
            {
                var l1 = new Label { Text = label, AutoSize = true, ForeColor = Color.FromArgb(139, 148, 158), Font = new Font("Segoe UI", 11F, FontStyle.Bold), Anchor = AnchorStyles.Left, Margin = new Padding(10) };
                var l2 = new Label { Text = val, AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 11F), Anchor = AnchorStyles.Left, Margin = new Padding(10) };
                pnlInfo.Controls.Add(l1); pnlInfo.Controls.Add(l2);
            }

            AddRow("Ad Soyad:", $"{_user.FirstName} {_user.LastName}");
            AddRow("Kimlik / TC:", _user.IdentityNumber);
            AddRow("E-Posta Adresi:", _user.Email);
            AddRow("Telefon No:", _user.Phone ?? "Belirtilmemiş");
            AddRow("Hesap Rolü:", roleText);

            _pnlContent.Controls.Add(pnlInfo);
            pnlInfo.BringToFront();
        }

        private void ShowTab_History()
        {
            SetActiveButton(_btnMenuHistory);
            ClearContent("📚  Okuma & Ödünç Geçmişi");

            var dgv = CreateGrid();
            dgv.Columns.Add("Type", "İşlem");
            dgv.Columns.Add("Book", "Kitap Adı");
            dgv.Columns.Add("Date", "Tarih");
            dgv.Columns.Add("Status", "Durum");

            // Okumalar (ReadingHistory)
            foreach (var rh in _user.ReadingHistories.OrderByDescending(r => r.LastReadDate))
            {
                dgv.Rows.Add("📖 Dijital Okuma", rh.Book?.Title ?? "-", rh.LastReadDate.ToString("dd.MM.yyyy HH:mm"), $"Sayfa/Konum: {rh.CurrentPage}");
            }

            // Ödünçler (Loans)
            foreach (var loan in _user.Loans.OrderByDescending(l => l.BorrowDate))
            {
                string status = loan.ReturnDate.HasValue ? "✅ İade Edildi" : "📤 Okunuyor";
                dgv.Rows.Add("📦 Fiziksel Ödünç", loan.Book?.Title ?? "-", loan.BorrowDate.ToString("dd.MM.yyyy"), status);
            }

            if (dgv.Rows.Count == 0) dgv.Rows.Add("-", "Geçmiş bulunmamaktadır.", "-", "-");
            
            _pnlContent.Controls.Add(dgv);
            dgv.BringToFront();
        }

        private void ShowTab_Favs()
        {
            SetActiveButton(_btnMenuFavs);
            ClearContent("❤️  Favorilerim");

            var dgv = CreateGrid();
            dgv.Columns.Add("Book", "Kitap Adı");
            dgv.Columns.Add("Author", "Yazar");
            dgv.Columns.Add("Date", "Favoriye Eklendiği Tarih");

            foreach (var fav in _user.Favorites.OrderByDescending(f => f.AddedDate))
            {
                dgv.Rows.Add(fav.Book?.Title ?? "-", fav.Book?.Author?.FullName ?? "-", fav.AddedDate.ToString("dd.MM.yyyy HH:mm"));
            }

            if (dgv.Rows.Count == 0) dgv.Rows.Add("-", "Henüz hiçbir kitabı favorilere eklemediniz.", "-");

            _pnlContent.Controls.Add(dgv);
            dgv.BringToFront();
        }

        private void ShowTab_Ratings()
        {
            SetActiveButton(_btnMenuRatings);
            ClearContent("⭐  Yaptığım Değerlendirmeler");

            var dgv = CreateGrid();
            dgv.Columns.Add("Book", "Kitap Adı");
            dgv.Columns.Add("Score", "Puan");
            dgv.Columns.Add("Review", "İnceleme / Yorum");
            
            dgv.Columns["Review"].FillWeight = 200;

            foreach (var rating in _user.Ratings.OrderByDescending(r => r.RatingDate))
            {
                string stars = new string('⭐', rating.Score) + new string('☆', 5 - rating.Score);
                dgv.Rows.Add(rating.Book?.Title ?? "-", stars, rating.Review);
            }

            if (dgv.Rows.Count == 0) dgv.Rows.Add("-", "-", "Hiçbir değerlendirme veya yorum yapmadınız.");

            _pnlContent.Controls.Add(dgv);
            dgv.BringToFront();
        }

        private DataGridView CreateGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(22, 27, 34),
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(35, 40, 58),
                RowHeadersVisible = false,
                ColumnHeadersHeight = 40,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                Margin = new Padding(0, 20, 0, 0)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 45);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(139, 148, 158);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(13, 17, 23);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(201, 209, 217);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 51, 59);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 27, 34);
            dgv.RowTemplate.Height = 40;
            return dgv;
        }

        // --- Fotoğraf Değiştir (Eski Kod Korundu) ---
        private async void BtnChangePhoto_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
            ofd.Title = "Profil Fotoğrafı Seç";
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string ext = Path.GetExtension(ofd.FileName);
                    string uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "profiles");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    string newFileName = $"profile_{_user.Id}_{Guid.NewGuid()}{ext}";
                    string newPath = Path.Combine(uploadsFolder, newFileName);
                    
                    File.Copy(ofd.FileName, newPath, true);
                    
                    using var db = new LibraryDbContext();
                    var userRepo = new UserRepository(db);
                    var dbUser = await userRepo.GetByIdAsync(_user.Id);
                    if (dbUser != null)
                    {
                        if (!string.IsNullOrEmpty(dbUser.ProfileImagePath) && File.Exists(dbUser.ProfileImagePath))
                        {
                            try { File.Delete(dbUser.ProfileImagePath); } catch { }
                        }
                        
                        dbUser.ProfileImagePath = newPath;
                        await userRepo.UpdateAsync(dbUser);
                        
                        _user.ProfileImagePath = newPath;
                        if (GlobalSession.CurrentUser != null) 
                            GlobalSession.CurrentUser.ProfileImagePath = newPath;
                        
                        _picProfile.ImageLocation = newPath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fotoğraf yüklenemedi:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
