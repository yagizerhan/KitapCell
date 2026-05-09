namespace KitapCell
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGiris = new System.Windows.Forms.TabPage();
            this.lblGirisKul = new System.Windows.Forms.Label();
            this.txtGirisKullanici = new System.Windows.Forms.TextBox();
            this.lblGirisSif = new System.Windows.Forms.Label();
            this.txtGirisSifre = new System.Windows.Forms.TextBox();
            this.chkAdmin = new System.Windows.Forms.CheckBox();
            this.btnGirisYap = new System.Windows.Forms.Button();
            this.lblDemo = new System.Windows.Forms.Label();
            this.tabKayit = new System.Windows.Forms.TabPage();
            this.lblKayKul = new System.Windows.Forms.Label();
            this.txtKayitKullanici = new System.Windows.Forms.TextBox();
            this.lblKayEma = new System.Windows.Forms.Label();
            this.txtKayitEmail = new System.Windows.Forms.TextBox();
            this.lblKaySif = new System.Windows.Forms.Label();
            this.txtKayitSifre = new System.Windows.Forms.TextBox();
            this.lblKaySifTek = new System.Windows.Forms.Label();
            this.txtKayitSifreTekrar = new System.Windows.Forms.TextBox();
            this.btnKayitOl = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabGiris.SuspendLayout();
            this.tabKayit.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(424, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📚 KitapCell";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSub
            // 
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(107)))), ((int)(((byte)(130)))));
            this.lblSub.Location = new System.Drawing.Point(0, 60);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(424, 22);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Hesabınıza giriş yapın veya kayıt olun";
            this.lblSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGiris);
            this.tabControl.Controls.Add(this.tabKayit);
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.ItemSize = new System.Drawing.Size(192, 36);
            this.tabControl.Location = new System.Drawing.Point(24, 96);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(392, 370);
            this.tabControl.TabIndex = 2;
            this.tabControl.TabIndex = 2;
            // 
            // tabGiris
            // 
            this.tabGiris.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(35)))));
            this.tabGiris.Controls.Add(this.lblGirisKul);
            this.tabGiris.Controls.Add(this.txtGirisKullanici);
            this.tabGiris.Controls.Add(this.lblGirisSif);
            this.tabGiris.Controls.Add(this.txtGirisSifre);
            this.tabGiris.Controls.Add(this.chkAdmin);
            this.tabGiris.Controls.Add(this.btnGirisYap);
            this.tabGiris.Controls.Add(this.lblDemo);
            this.tabGiris.Location = new System.Drawing.Point(4, 40);
            this.tabGiris.Name = "tabGiris";
            this.tabGiris.Size = new System.Drawing.Size(384, 326);
            this.tabGiris.TabIndex = 0;
            this.tabGiris.Text = "  Giriş Yap  ";
            // 
            // lblGirisKul
            // 
            this.lblGirisKul.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblGirisKul.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblGirisKul.Location = new System.Drawing.Point(16, 20);
            this.lblGirisKul.Name = "lblGirisKul";
            this.lblGirisKul.Size = new System.Drawing.Size(356, 18);
            this.lblGirisKul.TabIndex = 0;
            this.lblGirisKul.Text = "Kullanıcı Adı";
            // 
            // txtGirisKullanici
            // 
            this.txtGirisKullanici.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtGirisKullanici.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGirisKullanici.ForeColor = System.Drawing.Color.White;
            this.txtGirisKullanici.Location = new System.Drawing.Point(16, 44);
            this.txtGirisKullanici.Name = "txtGirisKullanici";
            this.txtGirisKullanici.Size = new System.Drawing.Size(356, 28);
            this.txtGirisKullanici.TabIndex = 1;
            // 
            // lblGirisSif
            // 
            this.lblGirisSif.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblGirisSif.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblGirisSif.Location = new System.Drawing.Point(16, 100);
            this.lblGirisSif.Name = "lblGirisSif";
            this.lblGirisSif.Size = new System.Drawing.Size(356, 18);
            this.lblGirisSif.TabIndex = 2;
            this.lblGirisSif.Text = "Şifre";
            // 
            // txtGirisSifre
            // 
            this.txtGirisSifre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtGirisSifre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGirisSifre.ForeColor = System.Drawing.Color.White;
            this.txtGirisSifre.Location = new System.Drawing.Point(16, 124);
            this.txtGirisSifre.Name = "txtGirisSifre";
            this.txtGirisSifre.PasswordChar = '●';
            this.txtGirisSifre.Size = new System.Drawing.Size(356, 28);
            this.txtGirisSifre.TabIndex = 3;
            // 
            // chkAdmin
            // 
            this.chkAdmin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAdmin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(185)))), ((int)(((byte)(215)))));
            this.chkAdmin.Location = new System.Drawing.Point(16, 180);
            this.chkAdmin.Name = "chkAdmin";
            this.chkAdmin.Size = new System.Drawing.Size(220, 22);
            this.chkAdmin.TabIndex = 4;
            this.chkAdmin.Text = "Admin olarak giriş yap";
            this.chkAdmin.UseVisualStyleBackColor = true;
            // 
            // btnGirisYap
            // 
            this.btnGirisYap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnGirisYap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGirisYap.FlatAppearance.BorderSize = 0;
            this.btnGirisYap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGirisYap.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGirisYap.ForeColor = System.Drawing.Color.White;
            this.btnGirisYap.Location = new System.Drawing.Point(16, 230);
            this.btnGirisYap.Name = "btnGirisYap";
            this.btnGirisYap.Size = new System.Drawing.Size(356, 42);
            this.btnGirisYap.TabIndex = 5;
            this.btnGirisYap.Text = "Giriş Yap";
            this.btnGirisYap.UseVisualStyleBackColor = false;
            this.btnGirisYap.Click += new System.EventHandler(this.BtnGirisYap_Click);
            // 
            // lblDemo
            // 
            this.lblDemo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDemo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(90)))), ((int)(((byte)(120)))));
            this.lblDemo.Location = new System.Drawing.Point(16, 282);
            this.lblDemo.Name = "lblDemo";
            this.lblDemo.Size = new System.Drawing.Size(356, 18);
            this.lblDemo.TabIndex = 6;
            this.lblDemo.Text = "Demo: herhangi bir kullanıcı adı/şifre girin";
            this.lblDemo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabKayit
            // 
            this.tabKayit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(35)))));
            this.tabKayit.Controls.Add(this.lblKayKul);
            this.tabKayit.Controls.Add(this.txtKayitKullanici);
            this.tabKayit.Controls.Add(this.lblKayEma);
            this.tabKayit.Controls.Add(this.txtKayitEmail);
            this.tabKayit.Controls.Add(this.lblKaySif);
            this.tabKayit.Controls.Add(this.txtKayitSifre);
            this.tabKayit.Controls.Add(this.lblKaySifTek);
            this.tabKayit.Controls.Add(this.txtKayitSifreTekrar);
            this.tabKayit.Controls.Add(this.btnKayitOl);
            this.tabKayit.Location = new System.Drawing.Point(4, 40);
            this.tabKayit.Name = "tabKayit";
            this.tabKayit.Size = new System.Drawing.Size(384, 326);
            this.tabKayit.TabIndex = 1;
            this.tabKayit.Text = "  Kayıt Ol  ";
            // 
            // lblKayKul
            // 
            this.lblKayKul.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKayKul.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKayKul.Location = new System.Drawing.Point(16, 16);
            this.lblKayKul.Name = "lblKayKul";
            this.lblKayKul.Size = new System.Drawing.Size(356, 18);
            this.lblKayKul.TabIndex = 0;
            this.lblKayKul.Text = "Kullanıcı Adı";
            // 
            // txtKayitKullanici
            // 
            this.txtKayitKullanici.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtKayitKullanici.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKayitKullanici.ForeColor = System.Drawing.Color.White;
            this.txtKayitKullanici.Location = new System.Drawing.Point(16, 40);
            this.txtKayitKullanici.Name = "txtKayitKullanici";
            this.txtKayitKullanici.Size = new System.Drawing.Size(356, 28);
            this.txtKayitKullanici.TabIndex = 1;
            // 
            // lblKayEma
            // 
            this.lblKayEma.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKayEma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKayEma.Location = new System.Drawing.Point(16, 88);
            this.lblKayEma.Name = "lblKayEma";
            this.lblKayEma.Size = new System.Drawing.Size(356, 18);
            this.lblKayEma.TabIndex = 2;
            this.lblKayEma.Text = "E-posta";
            // 
            // txtKayitEmail
            // 
            this.txtKayitEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtKayitEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKayitEmail.ForeColor = System.Drawing.Color.White;
            this.txtKayitEmail.Location = new System.Drawing.Point(16, 112);
            this.txtKayitEmail.Name = "txtKayitEmail";
            this.txtKayitEmail.Size = new System.Drawing.Size(356, 28);
            this.txtKayitEmail.TabIndex = 3;
            // 
            // lblKaySif
            // 
            this.lblKaySif.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKaySif.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKaySif.Location = new System.Drawing.Point(16, 160);
            this.lblKaySif.Name = "lblKaySif";
            this.lblKaySif.Size = new System.Drawing.Size(356, 18);
            this.lblKaySif.TabIndex = 4;
            this.lblKaySif.Text = "Şifre";
            // 
            // txtKayitSifre
            // 
            this.txtKayitSifre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtKayitSifre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKayitSifre.ForeColor = System.Drawing.Color.White;
            this.txtKayitSifre.Location = new System.Drawing.Point(16, 184);
            this.txtKayitSifre.Name = "txtKayitSifre";
            this.txtKayitSifre.PasswordChar = '●';
            this.txtKayitSifre.Size = new System.Drawing.Size(356, 28);
            this.txtKayitSifre.TabIndex = 5;
            // 
            // lblKaySifTek
            // 
            this.lblKaySifTek.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKaySifTek.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(155)))), ((int)(((byte)(200)))));
            this.lblKaySifTek.Location = new System.Drawing.Point(16, 232);
            this.lblKaySifTek.Name = "lblKaySifTek";
            this.lblKaySifTek.Size = new System.Drawing.Size(356, 18);
            this.lblKaySifTek.TabIndex = 6;
            this.lblKaySifTek.Text = "Şifre Tekrar";
            // 
            // txtKayitSifreTekrar
            // 
            this.txtKayitSifreTekrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.txtKayitSifreTekrar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKayitSifreTekrar.ForeColor = System.Drawing.Color.White;
            this.txtKayitSifreTekrar.Location = new System.Drawing.Point(16, 256);
            this.txtKayitSifreTekrar.Name = "txtKayitSifreTekrar";
            this.txtKayitSifreTekrar.PasswordChar = '●';
            this.txtKayitSifreTekrar.Size = new System.Drawing.Size(356, 28);
            this.txtKayitSifreTekrar.TabIndex = 7;
            // 
            // btnKayitOl
            // 
            this.btnKayitOl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnKayitOl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKayitOl.FlatAppearance.BorderSize = 0;
            this.btnKayitOl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKayitOl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnKayitOl.ForeColor = System.Drawing.Color.White;
            this.btnKayitOl.Location = new System.Drawing.Point(16, 268); // Adjusted slightly not to overflow
            this.btnKayitOl.Name = "btnKayitOl";
            this.btnKayitOl.Size = new System.Drawing.Size(356, 42);
            this.btnKayitOl.TabIndex = 8;
            this.btnKayitOl.Text = "Kayıt Ol";
            this.btnKayitOl.UseVisualStyleBackColor = false;
            this.btnKayitOl.Click += new System.EventHandler(this.BtnKayitOl_Click);

            // Ayar fix
            this.txtKayitSifreTekrar.Top = 230;
            this.lblKaySifTek.Top = 206;
            this.txtKayitSifre.Top = 158;
            this.lblKaySif.Top = 134;
            this.txtKayitEmail.Top = 86;
            this.lblKayEma.Top = 62;
            this.txtKayitKullanici.Top = 38;
            this.lblKayKul.Top = 14;

            // 
            // LoginForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(17)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(440, 520);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblSub);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KitapCell - Giriş / Kayıt";
            this.tabControl.ResumeLayout(false);
            this.tabGiris.ResumeLayout(false);
            this.tabGiris.PerformLayout();
            this.tabKayit.ResumeLayout(false);
            this.tabKayit.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGiris;
        private System.Windows.Forms.Label lblGirisKul;
        private System.Windows.Forms.TextBox txtGirisKullanici;
        private System.Windows.Forms.Label lblGirisSif;
        private System.Windows.Forms.TextBox txtGirisSifre;
        private System.Windows.Forms.CheckBox chkAdmin;
        private System.Windows.Forms.Button btnGirisYap;
        private System.Windows.Forms.Label lblDemo;
        private System.Windows.Forms.TabPage tabKayit;
        private System.Windows.Forms.Label lblKayKul;
        private System.Windows.Forms.TextBox txtKayitKullanici;
        private System.Windows.Forms.Label lblKayEma;
        private System.Windows.Forms.TextBox txtKayitEmail;
        private System.Windows.Forms.Label lblKaySif;
        private System.Windows.Forms.TextBox txtKayitSifre;
        private System.Windows.Forms.Label lblKaySifTek;
        private System.Windows.Forms.TextBox txtKayitSifreTekrar;
        private System.Windows.Forms.Button btnKayitOl;
    }
}
