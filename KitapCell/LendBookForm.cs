using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Repositories;
using KitapCell.Core;

namespace KitapCell
{
    /// <summary>
    /// Kitap ödünç verme işlemini gerçekleştiren iletişim kutusu (dialog) formu.
    /// Çağıran kod tarafından seçili kitap bilgisi iletilir; form üzerinden
    /// hangi üyeye ve hangi tarihe kadar ödünç verileceği belirlenir.
    /// Onay verildiğinde <see cref="BookLoan"/> kaydı oluşturulur ve
    /// kitabın mevcut kopya sayısı (<see cref="Book.AvailableCopies"/>) bir azaltılır.
    /// </summary>
    public partial class LendBookForm : Form
    {
        private LibraryDbContext _dbContext;
        private Repository<User> _userRepo;
        private Repository<BookLoan> _loanRepo;
        private Book _selectedBook;

        /// <summary>
        /// Formu başlatır; ödünç verilecek kitabı ve veritabanı bağlamını hazırlar.
        /// </summary>
        /// <param name="book">Ödünç verilecek kitap nesnesi.</param>
        public LendBookForm(Book book)
        {
            InitializeComponent();
            ThemeHelper.Apply(this);
            _selectedBook = book;
            
            _dbContext = new LibraryDbContext();
            _userRepo = new Repository<User>(_dbContext);
            _loanRepo = new Repository<BookLoan>(_dbContext);
            
            this.Load += LendBookForm_Load;
        }

        /// <summary>
        /// Form yüklendiğinde seçili kitap adını etiket üzerinde gösterir,
        /// varsayılan iade tarihini (<see cref="AppConfig.DefaultLoanDays"/>) ayarlar
        /// ve tüm aktif üyeleri açılır listeye yükler.
        /// </summary>
        private async void LendBookForm_Load(object sender, EventArgs e)
        {
            lblSeciliKitap.Text = "Seçili Kitap: " + _selectedBook.Title;
            dtpReturnDate.Value = DateTime.Now.AddDays(SettingsManager.Config.DefaultLoanDays);
            
            var users = await _userRepo.GetAllAsync();
            cmbUsers.DataSource = users.Select(u => new { Id = u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();
            cmbUsers.DisplayMember = "FullName";
            cmbUsers.ValueMember = "Id";
        }

        /// <summary>
        /// Seçilen üye ve tarih bilgileriyle ödünç kaydını oluşturur,
        /// kitabın mevcut kopya sayısını düşürür ve formu kapatır.
        /// </summary>
        private async void btnOnayla_Click(object sender, EventArgs e)
        {
            if (cmbUsers.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir üye seçiniz.", "Uyarı");
                return;
            }

            int userId = (int)cmbUsers.SelectedValue;
            
            var loan = new BookLoan
            {
                BookId = _selectedBook.Id,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = dtpReturnDate.Value,
                Status = KitapCell.Models.LoanStatus.Aktif
            };
            
            await _loanRepo.AddAsync(loan);
            
            var bookRepo = new BookRepository(_dbContext);
            var bookToUpdate = await bookRepo.GetByIdAsync(_selectedBook.Id);
            if(bookToUpdate != null)
            {
                bookToUpdate.AvailableCopies -= 1;
                await bookRepo.UpdateAsync(bookToUpdate);
            }

            MessageBox.Show("Kitap başarıyla ödünç verildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _dbContext?.Dispose();
        }
    }
}
