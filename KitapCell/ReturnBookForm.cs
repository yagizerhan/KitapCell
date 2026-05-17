using System;
using System.Linq;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Repositories;
using KitapCell.Core;

namespace KitapCell
{
    /// <summary>
    /// Dialog form that allows librarians to process book returns.
    /// Loads all currently active loan records into a grid; the librarian selects
    /// a row and clicks the return button to mark the loan as returned and
    /// restore the book's available copy count.
    /// </summary>
    public partial class ReturnBookForm : Form
    {
        /// <summary>Database context for this form's lifetime. Disposed in <see cref="OnFormClosed"/>.</summary>
        private LibraryDbContext _dbContext;

        /// <summary>Repository for reading and updating loan records.</summary>
        private LoanRepository _loanRepo;

        /// <summary>Repository for reading and updating book records (available copies).</summary>
        private BookRepository _bookRepo;

        /// <summary>The loan record selected by the librarian in the data grid.</summary>
        private BookLoan? _selectedLoan;

        /// <summary>
        /// Initialises the form, creates repository instances, and wires up the Load event.
        /// </summary>
        public ReturnBookForm()
        {
            InitializeComponent();
            _dbContext = new LibraryDbContext();
            _loanRepo  = new LoanRepository(_dbContext);
            _bookRepo  = new BookRepository(_dbContext);
            ThemeHelper.Apply(this);
            this.Load += ReturnBookForm_Load;
        }

        /// <summary>
        /// Loads all active loans (Active and Overdue) into the data grid on form load.
        /// Overdue rows are tinted red so the librarian can immediately identify late returns.
        /// </summary>
        private async void ReturnBookForm_Load(object sender, EventArgs e)
        {
            var loans = await _loanRepo.GetActiveLoansAsync();

            dgvLoans.Rows.Clear();
            foreach (var loan in loans)
            {
                bool gecikti = DateTime.Now > loan.DueDate;
                int rowIdx = dgvLoans.Rows.Add(
                    loan.Id,
                    loan.Book?.Title ?? "-",
                    $"{loan.User?.FirstName} {loan.User?.LastName}".Trim(),
                    loan.BorrowDate.ToString("dd.MM.yyyy"),
                    loan.DueDate.ToString("dd.MM.yyyy"),
                    gecikti ? "⚠️ Gecikmeli" : "✅ Aktif"
                );
                dgvLoans.Rows[rowIdx].Tag = loan;
                if (gecikti)
                    dgvLoans.Rows[rowIdx].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(248, 113, 113);
            }

            if (dgvLoans.Rows.Count == 0)
                lblBilgi.Text = "Şu an aktif ödünç kaydı bulunmamaktadır.";
            else
                lblBilgi.Text = $"{dgvLoans.Rows.Count} aktif ödünç kaydı listelendi.";
        }

        /// <summary>
        /// Updates the detail label and enables the Return button when the librarian
        /// selects a row in the data grid.
        /// </summary>
        private void DgvLoans_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLoans.SelectedRows.Count > 0 && dgvLoans.SelectedRows[0].Tag is BookLoan loan)
            {
                _selectedLoan = loan;
                lblSecili.Text = $"Seçili: {loan.Book?.Title} → {loan.User?.FirstName} {loan.User?.LastName}";
                btnIadeEt.Enabled = true;
            }
        }

        /// <summary>
        /// Confirms and processes the return of the selected loan record.
        /// Sets the loan status to <see cref="LoanStatus.IadeEdildi"/>, records the return date,
        /// and increments the book's <see cref="Book.AvailableCopies"/> by one.
        /// </summary>
        private async void btnIadeEt_Click(object sender, EventArgs e)
        {
            if (_selectedLoan == null) return;

            var confirm = MessageBox.Show(
                $"'{_selectedLoan.Book?.Title}' kitabını iade almak istediğinizden emin misiniz?",
                "İade Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Update the loan record to mark it as returned
            _selectedLoan.Status     = LoanStatus.IadeEdildi;
            _selectedLoan.ReturnDate = DateTime.Now;
            await _loanRepo.UpdateAsync(_selectedLoan);

            // Restore the available copy count on the book
            var book = await _bookRepo.GetByIdAsync(_selectedLoan.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
                await _bookRepo.UpdateAsync(book);
            }

            MessageBox.Show("İade işlemi başarıyla tamamlandı.", "İade Başarılı",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>Closes the form without processing a return.</summary>
        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>Disposes the database context when the form is closed.</summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _dbContext?.Dispose();
        }
    }
}
