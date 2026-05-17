using System;
using System.Drawing;
using System.Windows.Forms;
using KitapCell.Core;

namespace KitapCell
{
    /// <summary>
    /// The application's entry dialog that handles both login and registration.
    /// Displays two tabs: one for signing in with email + password,
    /// and one for creating a new member account.
    /// On successful authentication the <see cref="Core.GlobalSession.CurrentUser"/> is set
    /// and the form returns <see cref="System.Windows.Forms.DialogResult.OK"/>.
    /// </summary>
    public partial class LoginForm : Form
    {
        /// <summary>True when the authenticated user holds the Admin role. Set after successful login.</summary>
        public bool IsAdmin { get; private set; }

        /// <summary>Display name (first + last) of the authenticated user. Set after successful login.</summary>
        public string UserName { get; private set; } = "";

        public LoginForm()
        {
            InitializeComponent();
            try { this.Icon = new Icon(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico")); } catch { }
            ThemeHelper.Apply(this);
        }

        /// <summary>
        /// Persists the authenticated user's ID to a local file so that the
        /// application can restore the session on the next launch without forcing
        /// the user to log in again.
        /// </summary>
        private void SaveSession(int userId)
        {
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "login_session.txt");
                System.IO.File.WriteAllText(path, userId.ToString());
            }
            catch { }
        }

        /// <summary>
        /// Handles the Login button click.
        /// If the Admin shortcut checkbox is checked, bypasses password validation
        /// and logs in as the seeded admin account directly (demo convenience feature).
        /// Otherwise validates the entered email and password against the database.
        /// </summary>
        private async void BtnGirisYap_Click(object sender, EventArgs e)
        {
            using var db = new Data.LibraryDbContext();
            var userRepo = new Repositories.UserRepository(db);

            // Bypasses the password validation entirely if 'Admin' checkbox is checked (Demo mode requirement)
            if (chkAdmin.Checked)
            {
                var adminUser = await userRepo.GetByEmailAsync("admin@library.com");
                if (adminUser != null)
                {
                    Core.GlobalSession.CurrentUser = adminUser;
                    IsAdmin = true;
                    UserName = $"{adminUser.FirstName} {adminUser.LastName}".Trim();
                    SaveSession(adminUser.Id);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
            }

            string email = txtGirisKullanici.Text.Trim();
            string sifre = txtGirisSifre.Text;

            if (string.IsNullOrWhiteSpace(email))
            { MessageBox.Show("E-posta giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(sifre))
            { MessageBox.Show("Şifre giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var user = await userRepo.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                MessageBox.Show("Bu e-posta ile kayıtlı aktif hesap bulunamadı.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var hash = Services.PasswordHelper.Hash(sifre);
            if (user.PasswordHash != hash)
            {
                MessageBox.Show("Şifre hatalı.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Core.GlobalSession.CurrentUser = user;
            IsAdmin = user.Role == Models.UserRole.Admin;
            UserName = $"{user.FirstName} {user.LastName}".Trim();
            SaveSession(user.Id);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles the Register button click.
        /// Validates that all required fields are filled, passwords match, and the
        /// email is not already registered. On success, inserts the new user record
        /// and switches the tab control back to the Login tab.
        /// </summary>
        private async void BtnKayitOl_Click(object sender, EventArgs e)
        {
            string kulAdi = txtKayitKullanici.Text.Trim();
            string email = txtKayitEmail.Text.Trim();
            string sifre = txtKayitSifre.Text;
            string sifreTekrar = txtKayitSifreTekrar.Text;

            if (string.IsNullOrWhiteSpace(kulAdi) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre))
            { MessageBox.Show("Tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (sifre != sifreTekrar)
            { MessageBox.Show("Şifreler eşleşmiyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            using var db = new Data.LibraryDbContext();
            var userRepo = new Repositories.UserRepository(db);

            var existing = await userRepo.GetByEmailAsync(email);
            if (existing != null)
            { MessageBox.Show("Bu e-posta adresi zaten kayıtlı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var newUser = new Models.User
            {
                FirstName = kulAdi,
                LastName = "",
                Email = email,
                PasswordHash = Services.PasswordHelper.Hash(sifre),
                Role = Models.UserRole.Uye,
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            await userRepo.AddAsync(newUser);

            MessageBox.Show($"'{kulAdi}' kullanıcısı başarıyla kaydedildi!\nŞimdi giriş yapabilirsiniz.", "Kayıt Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tabControl.SelectedIndex = 0;
            txtGirisKullanici.Text = email;
        }
    }
}
