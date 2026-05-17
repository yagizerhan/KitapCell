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
    /// Dialog form for lending a book to a library member.
    /// The calling code passes the selected book; the librarian then chooses
    /// which member to lend it to and sets the due date.
    /// On confirmation a <see cref="Models.BookLoan"/> record is created and
    /// <see cref="Models.Book.AvailableCopies"/> is decremented by one.
    /// </summary>
    public partial class LendBookForm : Form
    {
        private LibraryDbContext _dbContext;
        private Repository<User> _userRepo;
        private Repository<BookLoan> _loanRepo;
        private Book _selectedBook;

        /// <summary>
        /// Initialises the form with the book to be lent and sets up the database context.
        /// </summary>
        /// <param name="book">The book entity that will be lent out.</param>
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
        /// Populates the form on load: displays the selected book title, sets the default
        /// due date from <see cref="Core.AppConfig.DefaultLoanDays"/>, and loads all
        /// active members into the user drop-down list.
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
        /// Confirms the loan transaction: creates the <see cref="Models.BookLoan"/> record,
        /// decrements the book's available copy count, and closes the dialog with OK.
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
