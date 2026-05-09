using System;
using System.Linq;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Repositories;
using KitapCell.Core;

namespace KitapCell
{
    public partial class ReturnBookForm : Form
    {
        private LibraryDbContext _dbContext;
        private LoanRepository _loanRepo;
        private BookRepository _bookRepo;
        private BookLoan? _selectedLoan;

        public ReturnBookForm()
        {
            InitializeComponent();
            _dbContext = new LibraryDbContext();
            _loanRepo  = new LoanRepository(_dbContext);
            _bookRepo  = new BookRepository(_dbContext);
            ThemeHelper.Apply(this);
            this.Load += ReturnBookForm_Load;
        }

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

        private void DgvLoans_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLoans.SelectedRows.Count > 0 && dgvLoans.SelectedRows[0].Tag is BookLoan loan)
            {
                _selectedLoan = loan;
                lblSecili.Text = $"Seçili: {loan.Book?.Title} → {loan.User?.FirstName} {loan.User?.LastName}";
                btnIadeEt.Enabled = true;
            }
        }

        private async void btnIadeEt_Click(object sender, EventArgs e)
        {
            if (_selectedLoan == null) return;

            var confirm = MessageBox.Show(
                $"'{_selectedLoan.Book?.Title}' kitabını iade almak istediğinizden emin misiniz?",
                "İade Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Loan güncelle
            _selectedLoan.Status     = LoanStatus.IadeEdildi;
            _selectedLoan.ReturnDate = DateTime.Now;
            await _loanRepo.UpdateAsync(_selectedLoan);

            // Kitap stoğunu artır
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

        private void btnKapat_Click(object sender, EventArgs e)
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
