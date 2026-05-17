using System;
using System.Windows.Forms;
using KitapCell.Services;
using KitapCell.Core;

namespace KitapCell
{
    /// <summary>
    /// Modal dialog that allows the currently logged-in user to submit or update
    /// a star rating (1–5) and an optional written review for a specific book.
    /// Calls <see cref="Services.BookService.AddOrUpdateBookRatingAsync"/> on save,
    /// which recalculates the book's average rating automatically.
    /// </summary>
    public partial class BookReviewForm : Form
    {
        /// <summary>ID of the book being reviewed.</summary>
        private int _bookId;

        /// <summary>ID of the currently logged-in user submitting the review.</summary>
        private int _userId;

        /// <summary>BookService instance used to persist the rating.</summary>
        private BookService _bookService;

        /// <summary>Currently selected star score (0 = nothing selected yet).</summary>
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

        /// <summary>
        /// Updates the five star buttons to reflect the selected score.
        /// Stars up to and including the given score show a filled star (⭐);
        /// the rest show an empty star (☆).
        /// </summary>
        /// <param name="score">The star rating to display (0–5).</param>
        private void SetupStars(int score)
        {
            _currentScore = score;
            btnStar1.Text = score >= 1 ? "⭐" : "☆";
            btnStar2.Text = score >= 2 ? "⭐" : "☆";
            btnStar3.Text = score >= 3 ? "⭐" : "☆";
            btnStar4.Text = score >= 4 ? "⭐" : "☆";
            btnStar5.Text = score >= 5 ? "⭐" : "☆";
        }

        /// <summary>
        /// Shared click handler for all five star buttons.
        /// Reads the numeric value from the button's Tag property and calls
        /// <see cref="SetupStars"/> to update the visual state.
        /// </summary>
        private void btnStar_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int s))
            {
                SetupStars(s);
            }
        }

        /// <summary>
        /// Saves the rating and optional review text to the database.
        /// At least one of score > 0 or a non-empty review is required.
        /// Closes the dialog with OK on success.
        /// </summary>
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
