using System;
using System.Windows.Forms;
using KitapCell.Services;
using KitapCell.Core;

namespace KitapCell
{
    public partial class BookReviewForm : Form
    {
        private int _bookId;
        private int _userId;
        private BookService _bookService;
        private int _currentScore = 0;

        public BookReviewForm(int bookId, string bookTitle)
        {
            InitializeComponent();
            ThemeHelper.Apply(this);
            
            _bookId = bookId;
            if (GlobalSession.CurrentUser != null)
                _userId = GlobalSession.CurrentUser.Id;
            
            lblBookTitle.Text = bookTitle;
            
            _bookService = new BookService(null!, null!, null!);
            SetupStars(0);
        }

        private void BookReviewForm_Load(object sender, EventArgs e)
        {
            // Mevcut veriler DBContext kullanılarak hızlıca doldurulabilir, ancak basitçe boş bırakıyoruz
        }

        private void SetupStars(int score)
        {
            _currentScore = score;
            btnStar1.Text = score >= 1 ? "⭐" : "☆";
            btnStar2.Text = score >= 2 ? "⭐" : "☆";
            btnStar3.Text = score >= 3 ? "⭐" : "☆";
            btnStar4.Text = score >= 4 ? "⭐" : "☆";
            btnStar5.Text = score >= 5 ? "⭐" : "☆";
        }

        private void btnStar_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int s))
            {
                SetupStars(s);
            }
        }

        private async void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentScore > 0 || !string.IsNullOrWhiteSpace(txtReview.Text))
                {
                    await _bookService.AddOrUpdateBookRatingAsync(_userId, _bookId, _currentScore, txtReview.Text);
                }

                MessageBox.Show("Değerlendirme başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
