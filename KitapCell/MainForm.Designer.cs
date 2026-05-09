namespace KitapCell
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.lblLogoSub = new System.Windows.Forms.Label();
            this.pnlUserArea = new System.Windows.Forms.Panel();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.btnLoginSidebar = new System.Windows.Forms.Button();
            this.pnlNavSep1 = new System.Windows.Forms.Panel();
            this.lblNavKutuphane = new System.Windows.Forms.Label();
            this.btnNavTumKitaplar = new System.Windows.Forms.Button();
            this.btnNavFavoriler = new System.Windows.Forms.Button();
            this.btnNavSonOkunanlar = new System.Windows.Forms.Button();
            this.btnNavEnCokOkunanlar = new System.Windows.Forms.Button();
            this.pnlNavSep2 = new System.Windows.Forms.Panel();
            this.lblNavYonetim = new System.Windows.Forms.Label();
            this.btnNavUyeler = new System.Windows.Forms.Button();
            this.btnNavOduncler = new System.Windows.Forms.Button();
            this.lblStatBooks = new System.Windows.Forms.Label();
            this.lblStatMembers = new System.Windows.Forms.Label();
            this.lblStatLoaned = new System.Windows.Forms.Label();
            this.pnlResizeHandle = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.flpToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnToolKitapEkle = new System.Windows.Forms.Button();
            this.btnToolSil = new System.Windows.Forms.Button();
            this.pnlRightSidebar = new System.Windows.Forms.Panel();
            this.pnlRightResizeHandle = new System.Windows.Forms.Panel();
            this.picDetailCover = new System.Windows.Forms.PictureBox();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblDetailAuthor = new System.Windows.Forms.Label();
            this.lblDetailInfo = new System.Windows.Forms.Label();
            this.btnCloseRightSidebar = new System.Windows.Forms.Button();
            this.btnToolGoruntule = new System.Windows.Forms.Button();
            this.btnToolOduncVer = new System.Windows.Forms.Button();
            this.btnToolIadeAl = new System.Windows.Forms.Button();
            this.btnToolRapor = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtContentSearch = new System.Windows.Forms.TextBox();
            this.btnViewList = new System.Windows.Forms.Button();
            this.btnViewGrid = new System.Windows.Forms.Button();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.dgvBooks = new System.Windows.Forms.DataGridView();
            this.flpGridBooks = new System.Windows.Forms.FlowLayoutPanel();
            this.colTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPublisher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCopies = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActions = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnRateBook = new System.Windows.Forms.Button();

            this.pnlSidebar.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDetailCover)).BeginInit();
            this.SuspendLayout();

            // MainForm
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Text = "KitapCell - Kütüphane Yönetim Sistemi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlResizeHandle);
            this.Controls.Add(this.pnlSidebar);

            // pnlResizeHandle – artıkssız sürükleme kulpu
            this.pnlResizeHandle.BackColor = System.Drawing.Color.FromArgb(35, 40, 58);
            this.pnlResizeHandle.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlResizeHandle.Width = 4;
            this.pnlResizeHandle.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.pnlResizeHandle.BringToFront();

            // pnlSidebar
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.MaximumSize = new System.Drawing.Size(240, 0);
            this.pnlSidebar.MinimumSize = new System.Drawing.Size(60, 0);
            this.pnlSidebar.Width = 240;
            this.pnlSidebar.AutoScroll = true;
            this.pnlSidebar.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblLogo, this.lblLogoSub, this.pnlUserArea,
                this.pnlNavSep1, this.lblNavKutuphane,
                this.btnNavTumKitaplar, this.lblStatBooks,
                this.btnNavFavoriler, this.btnNavSonOkunanlar, this.btnNavEnCokOkunanlar,
                this.pnlNavSep2, this.lblNavYonetim,
                this.btnNavUyeler, this.lblStatMembers,
                this.btnNavOduncler, this.lblStatLoaned,
                this.btnNavCikis
            });

            // lblLogo
            this.lblLogo.Text = "📚 KitapCell";
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.lblLogo.Location = new System.Drawing.Point(20, 20);
            this.lblLogo.Size = new System.Drawing.Size(200, 35);
            this.lblLogo.AutoSize = false;

            // lblLogoSub
            this.lblLogoSub.Text = "Kütüphane Yönetim Sistemi";
            this.lblLogoSub.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblLogoSub.ForeColor = System.Drawing.Color.FromArgb(100, 107, 130);
            this.lblLogoSub.Location = new System.Drawing.Point(20, 55);
            this.lblLogoSub.Size = new System.Drawing.Size(200, 18);

            // pnlUserArea
            this.pnlUserArea.BackColor = System.Drawing.Color.FromArgb(26, 30, 46);
            this.pnlUserArea.Location = new System.Drawing.Point(12, 82);
            this.pnlUserArea.Size = new System.Drawing.Size(216, 60);
            this.pnlUserArea.Padding = new System.Windows.Forms.Padding(8);
            
            this.picNavProfile = new System.Windows.Forms.PictureBox();
            this.picNavProfile.Size = new System.Drawing.Size(40, 40);
            this.picNavProfile.Location = new System.Drawing.Point(8, 10);
            this.picNavProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picNavProfile.BackColor = System.Drawing.Color.Transparent;
            this.picNavProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picNavProfile.Visible = false;

            this.pnlUserArea.Controls.AddRange(new System.Windows.Forms.Control[] { this.picNavProfile, this.lblUserName, this.lblUserRole, this.btnLoginSidebar });

            // lblUserName
            this.lblUserName.Text = "Misafir";
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(56, 8);
            this.lblUserName.Size = new System.Drawing.Size(140, 20);
            this.lblUserName.Visible = false;

            // lblUserRole
            this.lblUserRole.Text = "";
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.lblUserRole.Location = new System.Drawing.Point(56, 28);
            this.lblUserRole.Size = new System.Drawing.Size(140, 18);
            this.lblUserRole.Visible = false;

            // btnLoginSidebar
            this.btnLoginSidebar.Text = "Giriş Yap / Kayıt Ol";
            this.btnLoginSidebar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnLoginSidebar.ForeColor = System.Drawing.Color.White;
            this.btnLoginSidebar.BackColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.btnLoginSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoginSidebar.FlatAppearance.BorderSize = 0;
            this.btnLoginSidebar.Location = new System.Drawing.Point(8, 12);
            this.btnLoginSidebar.Size = new System.Drawing.Size(196, 35);
            this.btnLoginSidebar.Cursor = System.Windows.Forms.Cursors.Hand;

            // Separator 1
            this.pnlNavSep1.BackColor = System.Drawing.Color.FromArgb(35, 40, 58);
            this.pnlNavSep1.Location = new System.Drawing.Point(12, 155);
            this.pnlNavSep1.Size = new System.Drawing.Size(216, 1);

            // lblNavKutuphane
            this.lblNavKutuphane.Text = "KÜTÜPHANE";
            this.lblNavKutuphane.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblNavKutuphane.ForeColor = System.Drawing.Color.FromArgb(70, 80, 110);
            this.lblNavKutuphane.Location = new System.Drawing.Point(20, 165);
            this.lblNavKutuphane.Size = new System.Drawing.Size(200, 16);

            // Nav butonları - helper kurulum
            SetupNavButton(this.btnNavTumKitaplar, "Tüm Kitaplar", "📖", 190);
            SetupNavButton(this.btnNavFavoriler, "Favoriler", "⭐", 235);
            SetupNavButton(this.btnNavSonOkunanlar, "Son Okunanlar", "🕒", 280);
            
            SetupNavButton(this.btnNavEnCokOkunanlar, "Çok Okunanlar", "📕", 325);

            // Separator 2
            this.pnlNavSep2.BackColor = System.Drawing.Color.FromArgb(35, 40, 58);
            this.pnlNavSep2.Location = new System.Drawing.Point(12, 302);
            this.pnlNavSep2.Size = new System.Drawing.Size(216, 1);

            // lblNavYonetim
            this.lblNavYonetim.Text = "YÖNETİM";
            this.lblNavYonetim.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblNavYonetim.ForeColor = System.Drawing.Color.FromArgb(70, 80, 110);
            this.lblNavYonetim.Location = new System.Drawing.Point(20, 312);
            this.lblNavYonetim.Size = new System.Drawing.Size(200, 16);

            SetupNavButton(this.btnNavUyeler, "Üyeler", "👥", 335);
            SetupNavButton(this.btnNavOduncler, "Ödünçler", "💼", 371);

            this.btnNavCikis = new System.Windows.Forms.Button();
            SetupNavButton(this.btnNavCikis, "Çıkış Yap", "🚪", 736); 
            this.btnNavCikis.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnNavCikis.Visible = false;           
            // Sidebar istatistik etiketleri (sağ tarafta küçük sayılar)
            void SetupStatLabel(System.Windows.Forms.Label lbl, string value, int top, System.Drawing.Color color)
            {
                lbl.Text = value;
                lbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
                lbl.ForeColor = color;
                lbl.BackColor = System.Drawing.Color.FromArgb(color.R / 5, color.G / 5, color.B / 5);
                lbl.Location = new System.Drawing.Point(188, top + 10);
                lbl.Size = new System.Drawing.Size(36, 18);
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lbl.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            }
            SetupStatLabel(this.lblStatBooks,   "248", 190, System.Drawing.Color.FromArgb(99, 102, 241));
            SetupStatLabel(this.lblStatMembers, "124", 335, System.Drawing.Color.FromArgb(59, 130, 246));
            SetupStatLabel(this.lblStatLoaned,  "37",  371, System.Drawing.Color.FromArgb(34, 197, 94));

            // pnlSidebar içerisinden Ayarlar ve Çıkış yap temizlendi.
            // pnlRightSidebar
            this.pnlRightSidebar.BackColor = System.Drawing.Color.FromArgb(18, 22, 29);
            this.pnlRightSidebar.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRightSidebar.Width = 240;
            this.pnlRightSidebar.Visible = false;
            this.pnlRightSidebar.Padding = new System.Windows.Forms.Padding(16, 10, 16, 16);

            // pnlRightResizeHandle
            this.pnlRightResizeHandle.BackColor = System.Drawing.Color.FromArgb(35, 40, 58);
            this.pnlRightResizeHandle.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.pnlRightResizeHandle.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRightResizeHandle.Width = 4;
            this.pnlRightResizeHandle.Visible = false;
            
            this.btnCloseRightSidebar.Text = "✕";
            this.btnCloseRightSidebar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCloseRightSidebar.ForeColor = System.Drawing.Color.FromArgb(139, 148, 158);
            this.btnCloseRightSidebar.BackColor = System.Drawing.Color.Transparent;
            this.btnCloseRightSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseRightSidebar.FlatAppearance.BorderSize = 0;
            this.btnCloseRightSidebar.Size = new System.Drawing.Size(40, 40);
            this.btnCloseRightSidebar.Location = new System.Drawing.Point(275, 5);
            this.btnCloseRightSidebar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloseRightSidebar.Click += (s, e) => { this.pnlRightSidebar.Visible = false; this.pnlRightResizeHandle.Visible = false; };

            this.picDetailCover.BackColor = System.Drawing.Color.FromArgb(18, 22, 29);
            this.picDetailCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDetailCover.Dock = System.Windows.Forms.DockStyle.Top;
            this.picDetailCover.Height = 180;
            this.picDetailCover.TabStop = false;
            
            this.lblDetailTitle.Text = "-";
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.ForeColor = System.Drawing.Color.White;
            this.lblDetailTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDetailTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDetailTitle.Height = 70;

            this.lblDetailAuthor.Text = "-";
            this.lblDetailAuthor.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDetailAuthor.ForeColor = System.Drawing.Color.FromArgb(139, 148, 158);
            this.lblDetailAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDetailAuthor.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDetailAuthor.Height = 40;

            var sepR = new System.Windows.Forms.Panel { BackColor = System.Drawing.Color.FromArgb(48, 54, 61), Dock = System.Windows.Forms.DockStyle.Top, Height = 1 };

            this.lblDetailInfo.Text = "-";
            this.lblDetailInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDetailInfo.ForeColor = System.Drawing.Color.FromArgb(201, 209, 217);
            this.lblDetailInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDetailInfo.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.lblDetailInfo.Height = 250;

            this.btnRateBook.Text = "⭐ Değerlendir";
            this.btnRateBook.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRateBook.ForeColor = System.Drawing.Color.White;
            this.btnRateBook.BackColor = System.Drawing.Color.FromArgb(245, 158, 11);
            this.btnRateBook.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRateBook.FlatAppearance.BorderSize = 0;
            this.btnRateBook.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRateBook.Height = 45;
            this.btnRateBook.Cursor = System.Windows.Forms.Cursors.Hand;

            this.pnlRightSidebar.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnRateBook, this.lblDetailInfo, sepR, this.lblDetailAuthor, this.lblDetailTitle, this.picDetailCover, this.btnCloseRightSidebar
            });

            // pnlMain
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Controls.Add(this.pnlRightSidebar);
            this.pnlMain.Controls.Add(this.pnlRightResizeHandle);
            this.pnlMain.Controls.Add(this.pnlContent);
               // flpToolbar
            this.flpToolbar.Location = new System.Drawing.Point(0, 0);
            this.flpToolbar.Size = new System.Drawing.Size(920, 104);
            this.flpToolbar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.flpToolbar.BackColor = System.Drawing.Color.Transparent;
            this.flpToolbar.WrapContents = false;
            this.flpToolbar.AutoScroll = true;
            this.flpToolbar.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.btnToolProfil = new System.Windows.Forms.Button();
            this.btnToolSunucu = new System.Windows.Forms.Button();
            this.btnToolAyarlar = new System.Windows.Forms.Button();

            this.flpToolbar.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnToolKitapEkle, this.btnToolSil,
                this.btnToolGoruntule, this.btnToolOduncVer, this.btnToolIadeAl, this.btnToolRapor,
                this.btnToolProfil, this.btnToolSunucu, this.btnToolAyarlar
            });

            void SetupToolBtn(System.Windows.Forms.Button btn, string icon, string label,
                System.Drawing.Color accentColor)
            {
                // btn.Text BOŞ bırakıyoruz — her şeyi Paint'te çiziyoruz
                // Böylece Windows'un kendi render'ı devreye GİRMİYOR (çift yazı sorunu yok)
                btn.Text      = "";
                btn.Tag       = icon + "|" + label; // ikon ve etiketi Tag'da saklıyoruz
                btn.Font      = new System.Drawing.Font("Segoe UI", 9F);
                btn.Size      = new System.Drawing.Size(120, 96);
                btn.ForeColor = System.Drawing.Color.FromArgb(180, 190, 210);
                btn.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.FlatAppearance.BorderSize  = 0;
                btn.FlatAppearance.BorderColor = accentColor;
                btn.Cursor    = System.Windows.Forms.Cursors.Hand;
                btn.Margin    = new System.Windows.Forms.Padding(2, 4, 2, 4);

                btn.Paint += (s, pe) =>
                {
                    var parts = btn.Tag?.ToString()?.Split('|');
                    if (parts == null || parts.Length < 2) return;
                    string ikon  = parts[0];
                    string lbl   = parts[1];

                    var g = pe.Graphics;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    // Alt 3px accent çizgisi
                    using var accentPen = new System.Drawing.Pen(accentColor, 3);
                    g.DrawLine(accentPen, 4, btn.Height - 3, btn.Width - 4, btn.Height - 3);

                    // İkon — Image olarak (Ortalama hatasını gidermek ve pürüzsüzleştirmek için)
                    using (var img = CreateEmojiImage(ikon, 42))
                    {
                        int imgX = (btn.Width - 42) / 2;
                        int imgY = 8;
                        g.DrawImage(img, imgX, imgY, 42, 42);
                    }

                    // Etiket — otomatik küçülen font (8.5F'ten 11.5F'e büyütüldü)
                    using var lblBrush = new System.Drawing.SolidBrush(btn.ForeColor);
                    var lblRect = new System.Drawing.RectangleF(2, btn.Height - 32, btn.Width - 4, 26);
                    float fontSize = 11.5F;
                    using var lblFontLarge = new System.Drawing.Font("Segoe UI", fontSize, System.Drawing.FontStyle.Bold);
                    var measured = g.MeasureString(lbl, lblFontLarge);
                    if (measured.Width > lblRect.Width)
                        fontSize = 9.5F;
                    
                    var centerFmt = new System.Drawing.StringFormat
                    {
                         Alignment = System.Drawing.StringAlignment.Center,
                         LineAlignment = System.Drawing.StringAlignment.Center
                    };
                    using var lblFont = new System.Drawing.Font("Segoe UI", fontSize, System.Drawing.FontStyle.Bold);
                    g.DrawString(lbl, lblFont, lblBrush, lblRect, centerFmt);
                };

                // Hover efekti
                var origBack = btn.BackColor;
                var hoverBack = System.Drawing.Color.FromArgb(
                    Math.Min(accentColor.R / 5 + 22, 255),
                    Math.Min(accentColor.G / 5 + 22, 255),
                    Math.Min(accentColor.B / 5 + 22, 255));
                btn.MouseEnter += (s, e) => { btn.BackColor = hoverBack; btn.ForeColor = System.Drawing.Color.White;  btn.Invalidate(); };
                btn.MouseLeave += (s, e) => { btn.BackColor = origBack;  btn.ForeColor = System.Drawing.Color.FromArgb(180, 190, 210); btn.Invalidate(); };
            }

            var indigo  = System.Drawing.Color.FromArgb( 99, 102, 241);
            var danger  = System.Drawing.Color.FromArgb(239,  68,  68);
            var cyan    = System.Drawing.Color.FromArgb( 34, 211, 238);
            var success = System.Drawing.Color.FromArgb( 34, 197,  94);
            var amber   = System.Drawing.Color.FromArgb(245, 158,  11);
            var blue    = System.Drawing.Color.FromArgb( 59, 130, 246);

            SetupToolBtn(this.btnToolKitapEkle, "📚", "Kitap Ekle",  indigo);
            SetupToolBtn(this.btnToolSil,       "🗑️",  "Sil",         danger);
            SetupToolBtn(this.btnToolGoruntule, "👁️",  "Görüntüle",   cyan);
            SetupToolBtn(this.btnToolOduncVer,  "💼", "Ödünç Ver",   success);
            SetupToolBtn(this.btnToolIadeAl,    "📥", "İade Al",     amber);
            SetupToolBtn(this.btnToolRapor,     "📊", "Raporlar",    blue);
            
            SetupToolBtn(this.btnToolProfil,    "👤", "Profilim",    cyan);
            SetupToolBtn(this.btnToolSunucu,    "🌐", "Sunucu",      indigo);
            SetupToolBtn(this.btnToolAyarlar,   "⚙️", "Ayarlar",     amber);

            this.btnToolProfil.Visible  = false;
            this.btnToolSunucu.Visible  = false;
            this.btnToolAyarlar.Visible = false;

            // Ayırıcı çizgi
            var sep1 = new System.Windows.Forms.Panel();
            sep1.BackColor = System.Drawing.Color.FromArgb(48, 54, 61);
            sep1.Size = new System.Drawing.Size(1, 60);
            sep1.Margin = new System.Windows.Forms.Padding(6, 14, 6, 14);
            this.flpToolbar.Controls.Add(sep1);

            // pnlHeader (Toolbar ve Arama kontrol kapsayıcısı)
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 190;
            this.pnlHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlHeader.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtContentSearch, this.btnViewList, this.btnViewGrid, this.flpToolbar, this.lblPageTitle
            });

            // pnlContent - arama kutusu, toolbar ve grid
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Padding = new System.Windows.Forms.Padding(5, 20, 0, 20);
            this.pnlContent.AutoScroll = false;
            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.flpGridBooks, this.dgvBooks, this.pnlHeader
            });

            // txtContentSearch (Yalnız arama kutusu)
            this.txtContentSearch.Location = new System.Drawing.Point(0, 140);
            this.txtContentSearch.Size = new System.Drawing.Size(500, 34);
            this.txtContentSearch.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.txtContentSearch.ForeColor = System.Drawing.Color.White;
            this.txtContentSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtContentSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.txtContentSearch.PlaceholderText = "🔍  Kitap, yazar veya kategori ara...";

            // btnViewList
            this.btnViewList.Location = new System.Drawing.Point(510, 140);
            this.btnViewList.Size = new System.Drawing.Size(40, 34);
            this.btnViewList.Text = "☰";
            this.btnViewList.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnViewList.BackColor = System.Drawing.Color.FromArgb(99, 102, 241); // Aktif secili renk
            this.btnViewList.ForeColor = System.Drawing.Color.White;
            this.btnViewList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewList.FlatAppearance.BorderSize = 0;
            this.btnViewList.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnViewGrid
            this.btnViewGrid.Location = new System.Drawing.Point(555, 140);
            this.btnViewGrid.Size = new System.Drawing.Size(40, 34);
            this.btnViewGrid.Text = "🔲";
            this.btnViewGrid.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnViewGrid.BackColor = System.Drawing.Color.FromArgb(35, 40, 58); // Pasif renk
            this.btnViewGrid.ForeColor = System.Drawing.Color.FromArgb(201, 209, 217);
            this.btnViewGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewGrid.FlatAppearance.BorderSize = 0;
            this.btnViewGrid.Cursor = System.Windows.Forms.Cursors.Hand;

            // lblPageTitle - arama altina alindi
            this.lblPageTitle.Text = "Tüm Kitaplar";
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(0, 95);
            this.lblPageTitle.Size = new System.Drawing.Size(400, 40);
            this.lblPageTitle.AutoSize = false;

            // dgvBooks (Liste Görünümü)
            this.dgvBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBooks.BackgroundColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.dgvBooks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBooks.GridColor = System.Drawing.Color.FromArgb(35, 40, 58);
            this.dgvBooks.ColumnHeadersHeight = 50;
            this.dgvBooks.RowTemplate.Height = 55;
            this.dgvBooks.AllowUserToResizeRows = false;
            this.dgvBooks.AllowUserToResizeColumns = true;
            this.dgvBooks.AllowUserToAddRows = false;
            this.dgvBooks.AllowUserToDeleteRows = false;
            this.dgvBooks.ReadOnly = true;
            this.dgvBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBooks.MultiSelect = false;
            this.dgvBooks.RowHeadersVisible = false;
            this.dgvBooks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // Header stilleri
            this.dgvBooks.EnableHeadersVisualStyles = false; // Klasik Windows 98 tarzı header'ı kapatır
            this.dgvBooks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal; // Sadece yatay çizgi (web tarzı)
            this.dgvBooks.AdvancedColumnHeadersBorderStyle.All = System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.None; // Beyaz çizgi/bar problemini kesin çözer
            this.dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(33, 38, 45); // --bg-table-header
            this.dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 148, 158); // --text-secondary
            this.dgvBooks.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.dgvBooks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvBooks.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvBooks.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(33, 38, 45);
            this.dgvBooks.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(139, 148, 158);

            // Row stilleri
            this.dgvBooks.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(13, 17, 23); // --bg-dark
            this.dgvBooks.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(201, 209, 217); // --text-primary
            this.dgvBooks.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.dgvBooks.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(45, 51, 59); // --bg-row-selected
            this.dgvBooks.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(201, 209, 217); // --text-primary
            this.dgvBooks.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(22, 27, 34); // --bg-sidebar (alternatif)
            this.dgvBooks.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0); // Daha fazla web padding'i
            this.dgvBooks.GridColor = System.Drawing.Color.FromArgb(48, 54, 61); // --border-color

            // Sütunlar
            this.colTitle.Name = "colTitle"; this.colTitle.HeaderText = "BAŞLIK"; this.colTitle.FillWeight = 200;
            this.colAuthor.Name = "colAuthor"; this.colAuthor.HeaderText = "YAZAR"; this.colAuthor.FillWeight = 130;
            this.colCategory.Name = "colCategory"; this.colCategory.HeaderText = "KATEGORİ"; this.colCategory.FillWeight = 100;
            this.colPublisher.Name = "colPublisher"; this.colPublisher.HeaderText = "YAYINCI"; this.colPublisher.FillWeight = 110;
            this.colYear.Name = "colYear"; this.colYear.HeaderText = "YIL"; this.colYear.FillWeight = 60;
            this.colCopies.Name = "colCopies"; this.colCopies.HeaderText = "KOPYA"; this.colCopies.FillWeight = 70; this.colCopies.Visible = false;
            this.colStatus.Name = "colStatus"; this.colStatus.HeaderText = "DURUM"; this.colStatus.FillWeight = 85;
            this.colActions.Name = "colActions"; this.colActions.HeaderText = ""; this.colActions.Text = "Düzenle"; this.colActions.UseColumnTextForButtonValue = true; this.colActions.FillWeight = 70;
            this.colActions.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(40, 45, 65);
            this.colActions.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(180, 180, 220);
            this.colActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this.dgvBooks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colTitle, this.colAuthor, this.colCategory, this.colPublisher,
                this.colCopies, this.colStatus
            });

            // flpGridBooks (Kare / Izgara Görünümü)
            this.flpGridBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpGridBooks.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.flpGridBooks.AutoScroll = true;
            this.flpGridBooks.Visible = false; // Başlangıçta gizli
            this.flpGridBooks.Padding = new System.Windows.Forms.Padding(0);
            this.flpGridBooks.Margin = new System.Windows.Forms.Padding(0);

            this.pnlSidebar.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDetailCover)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupNavButton(System.Windows.Forms.Button btn, string text, string emoji, int top)
        {
            btn.Text = "   " + text;
            if (!string.IsNullOrEmpty(emoji)) {
                btn.Image = CreateEmojiImage(emoji, 26);
                btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            }
            btn.Font = new System.Drawing.Font("Segoe UI", 15.5F, System.Drawing.FontStyle.Bold);
            btn.ForeColor = System.Drawing.Color.White;
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(41, 46, 56);
            btn.Location = new System.Drawing.Point(8, top);
            btn.Size = new System.Drawing.Size(224, 38);
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btn.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void SetupStatCard(System.Windows.Forms.Panel card, int index, string emoji, string label, string value, System.Drawing.Color accentColor)
        {
            int cardWidth = 230;
            int gap = 12;
            card.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            card.Location = new System.Drawing.Point(index * (cardWidth + gap), 0);
            card.Size = new System.Drawing.Size(cardWidth, 95);

            var lblEmoji = new System.Windows.Forms.Label();
            lblEmoji.Text = "";
            lblEmoji.Image = CreateEmojiImage(emoji, 32);
            lblEmoji.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblEmoji.Location = new System.Drawing.Point(12, 15);
            lblEmoji.Size = new System.Drawing.Size(44, 44);
            lblEmoji.AutoSize = false;

            var lblVal = new System.Windows.Forms.Label();
            lblVal.Text = value;
            lblVal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            lblVal.ForeColor = accentColor;
            lblVal.Location = new System.Drawing.Point(64, 12);
            lblVal.Size = new System.Drawing.Size(150, 38);
            lblVal.AutoSize = false;

            var lblLbl = new System.Windows.Forms.Label();
            lblLbl.Text = label;
            lblLbl.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblLbl.ForeColor = System.Drawing.Color.FromArgb(120, 125, 160);
            lblLbl.Location = new System.Drawing.Point(64, 50);
            lblLbl.Size = new System.Drawing.Size(160, 20);
            lblLbl.AutoSize = false;

            var pnlAccent = new System.Windows.Forms.Panel();
            pnlAccent.BackColor = accentColor;
            pnlAccent.Location = new System.Drawing.Point(0, 0);
            pnlAccent.Size = new System.Drawing.Size(4, 95);

            card.Controls.AddRange(new System.Windows.Forms.Control[] { pnlAccent, lblEmoji, lblVal, lblLbl });
        }

        // Controls
        private System.Windows.Forms.Panel pnlResizeHandle;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Label lblLogoSub;
        private System.Windows.Forms.Panel pnlUserArea;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Button btnLoginSidebar;
        private System.Windows.Forms.Panel pnlNavSep1;
        private System.Windows.Forms.Label lblNavKutuphane;
        private System.Windows.Forms.Button btnNavTumKitaplar;
        private System.Windows.Forms.Label lblStatBooks;   // sidebar sayac
        private System.Windows.Forms.Button btnNavFavoriler;
        private System.Windows.Forms.Button btnNavSonOkunanlar;
        private System.Windows.Forms.Button btnNavEnCokOkunanlar;
        private System.Windows.Forms.Panel pnlNavSep2;
        private System.Windows.Forms.Label lblNavYonetim;
        private System.Windows.Forms.Button btnNavUyeler;
        private System.Windows.Forms.Label lblStatMembers; // sidebar sayac
        private System.Windows.Forms.Button btnNavOduncler;
        private System.Windows.Forms.Label lblStatLoaned;  // sidebar sayac
        private System.Windows.Forms.Panel pnlNavSep3;
        private System.Windows.Forms.Button btnToolAyarlar;
        private System.Windows.Forms.Button btnToolSunucu;
        private System.Windows.Forms.Button btnToolProfil;
        private System.Windows.Forms.Button btnNavCikis;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.FlowLayoutPanel flpToolbar;
        private System.Windows.Forms.Button btnToolKitapEkle;
        private System.Windows.Forms.Button btnToolSil;
        private System.Windows.Forms.Button btnToolGoruntule;
        private System.Windows.Forms.Button btnToolOduncVer;
        private System.Windows.Forms.Button btnToolIadeAl;
        private System.Windows.Forms.Button btnToolRapor;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TextBox txtContentSearch;
        private System.Windows.Forms.Button btnViewList;
        private System.Windows.Forms.Button btnViewGrid;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.DataGridView dgvBooks;
        private System.Windows.Forms.FlowLayoutPanel flpGridBooks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPublisher;
        private System.Windows.Forms.DataGridViewTextBoxColumn colYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCopies;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewButtonColumn colActions;
        private System.Windows.Forms.Button btnRateBook;
        private System.Windows.Forms.Panel pnlRightSidebar;
        private System.Windows.Forms.PictureBox picDetailCover;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblDetailAuthor;
        private System.Windows.Forms.Label lblDetailInfo;
        private System.Windows.Forms.Button btnCloseRightSidebar;
        private System.Windows.Forms.Panel pnlRightResizeHandle;
        private System.Windows.Forms.PictureBox picNavProfile;
        #endregion
    }
}
