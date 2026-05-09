using System;
using System.Threading.Tasks;
using KitapCell.Models;
using KitapCell.Repositories;
using System.Linq;
using KitapCell.Data;

namespace KitapCell.Services
{
    /// <summary>
    /// Contains all business logic related to books.
    /// Enforces rules for lending, returning, rating, and reading-progress tracking.
    /// Delegates all database operations to the appropriate Repository classes.
    /// </summary>
    public class BookService
    {
        private readonly BookRepository _bookRepository;
        private readonly LoanRepository _loanRepository;
        private readonly UserService _userService;

        /// <summary>
        /// Receives required repository and service objects via dependency injection.
        /// </summary>
        public BookService(BookRepository bookRepository, LoanRepository loanRepository, UserService userService)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _userService = userService;
        }

        /// <summary>
        /// Lends the specified book to a user.
        /// Business rules:
        /// <list type="bullet">
        ///   <item>The book must exist.</item>
        ///   <item>At least one copy must be available (<see cref="Book.AvailableCopies"/> &gt; 0).</item>
        /// </list>
        /// On success, decrements the available copy count and creates a new <see cref="BookLoan"/> record.
        /// </summary>
        /// <param name="bookId">ID of the book to lend.</param>
        /// <param name="userId">ID of the user borrowing the book.</param>
        /// <returns>A tuple containing a success flag and a message for the UI.</returns>
        public async Task<(bool Success, string Message)> LoanBookAsync(int bookId, int userId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null) return (false, "Book not found.");
            if (book.AvailableCopies <= 0) return (false, "No copies available on the shelf.");

            // Additional business rules (e.g. max active loans per user) can be added here
            var loan = new BookLoan
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14), // Default loan period: 14 days
                Status = LoanStatus.Aktif
            };

            book.AvailableCopies -= 1;

            await _loanRepository.AddAsync(loan);
            await _bookRepository.UpdateAsync(book);

            return (true, "Book lent successfully.");
        }

        /// <summary>
        /// Processes the return of a loaned book.
        /// Business rules:
        /// <list type="bullet">
        ///   <item>The loan record must exist and must not already be returned.</item>
        ///   <item>If the return date exceeds <see cref="BookLoan.DueDate"/>, a late-return
        ///   penalty is applied and the user's reputation score is reduced.</item>
        ///   <item>On-time returns award reputation points.</item>
        /// </list>
        /// </summary>
        /// <param name="loanId">ID of the loan record to return.</param>
        /// <returns>A tuple containing a success flag and a message (including penalty info if applicable).</returns>
        public async Task<(bool Success, string Message)> ReturnBookAsync(int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null || loan.Status == LoanStatus.IadeEdildi)
                return (false, "Invalid operation or book is already returned.");

            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
                await _bookRepository.UpdateAsync(book);
            }

            loan.ReturnDate = DateTime.Now;

            if (loan.ReturnDate > loan.DueDate)
            {
                // Late return: calculate penalty and reduce reputation
                loan.Status = LoanStatus.Gecikti;
                int delayDays = (loan.ReturnDate.Value - loan.DueDate).Days;
                loan.PenaltyAmount = delayDays * 5;
                await _userService.UpdateReputationAsync(loan.UserId, -10);
            }
            else
            {
                // On-time return: reward with reputation points
                loan.Status = LoanStatus.IadeEdildi;
                await _userService.UpdateReputationAsync(loan.UserId, +5);
            }

            await _loanRepository.UpdateAsync(loan);

            if (loan.PenaltyAmount > 0)
                return (true, $"Book returned. Late penalty applied: {loan.PenaltyAmount} TL");

            return (true, "Book returned on time. Thank you!");
        }

        /// <summary>
        /// Adds or updates a user's star rating and written review for a book.
        /// Recalculates the book's average rating (<see cref="Book.AverageRating"/>) after saving.
        /// </summary>
        /// <param name="userId">ID of the user submitting the rating.</param>
        /// <param name="bookId">ID of the book being rated.</param>
        /// <param name="score">Star score from 1 to 5.</param>
        /// <param name="review">Optional written review text.</param>
        /// <returns>A tuple containing a success flag and a result message.</returns>
        public async Task<(bool Success, string Message)> AddOrUpdateBookRatingAsync(
            int userId, int bookId, int score, string review)
        {
            using var context = new LibraryDbContext();

            // Update existing rating if found, otherwise insert a new one
            var existingRating = context.Set<UserRating>()
                .FirstOrDefault(r => r.UserId == userId && r.BookId == bookId);

            if (existingRating != null)
            {
                existingRating.Score = score;
                existingRating.Review = review;
                existingRating.RatingDate = DateTime.Now;
                context.Set<UserRating>().Update(existingRating);
            }
            else
            {
                var newRating = new UserRating
                {
                    UserId = userId,
                    BookId = bookId,
                    Score = score,
                    Review = review,
                    RatingDate = DateTime.Now
                };
                await context.Set<UserRating>().AddAsync(newRating);
            }
            await context.SaveChangesAsync();

            // Recalculate the book's average rating across all reviews
            var allRatings = context.Set<UserRating>().Where(r => r.BookId == bookId).ToList();
            var book = await context.Set<Book>().FindAsync(bookId);
            if (book != null && allRatings.Any())
            {
                book.AverageRating = (float)Math.Round(allRatings.Average(r => r.Score), 1);
                context.Set<Book>().Update(book);
                await context.SaveChangesAsync();
            }

            return (true, "Rating saved successfully.");
        }

        /// <summary>
        /// Saves the user's reading progress (current PDF page number) for a book.
        /// Creates a new reading history entry if one does not yet exist; otherwise updates it.
        /// This data is used by <c>BookReaderForm</c> to resume reading from the last position.
        /// </summary>
        /// <param name="userId">ID of the reading user.</param>
        /// <param name="bookId">ID of the book being read.</param>
        /// <param name="currentPage">The page number the user is currently on.</param>
        /// <returns>A tuple containing a success flag and a result message.</returns>
        public async Task<(bool Success, string Message)> UpdateReadingProgressAsync(
            int userId, int bookId, int currentPage)
        {
            using var context = new LibraryDbContext();
            var history = context.Set<ReadingHistory>()
                .FirstOrDefault(h => h.UserId == userId && h.BookId == bookId);

            if (history != null)
            {
                history.CurrentPage = currentPage;
                history.IsCompleted = false; // Completion logic can be extended later
                context.Set<ReadingHistory>().Update(history);
            }
            else
            {
                var newHistory = new ReadingHistory
                {
                    UserId = userId,
                    BookId = bookId,
                    CurrentPage = currentPage,
                    StartDate = DateTime.Now
                };
                await context.Set<ReadingHistory>().AddAsync(newHistory);
            }

            await context.SaveChangesAsync();
            return (true, "Reading progress saved.");
        }
    }
}
