using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KitapCell.Models;
using KitapCell.Data;

namespace KitapCell.Repositories
{
    /// <summary>
    /// Handles database queries specific to the Book entity.
    /// Advanced queries such as search, favorite management, and reading-history
    /// statistics are implemented here; basic CRUD is inherited from
    /// <see cref="Repository{T}"/>.
    /// </summary>
    public class BookRepository : Repository<Book>
    {
        public BookRepository(LibraryDbContext context) : base(context) { }

        /// <summary>
        /// Returns all books with their author and category eagerly loaded.
        /// Used for the main book-list screen.
        /// </summary>
        public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Returns the book with the specified ID together with its author,
        /// category, and ratings. Used for the book detail screen.
        /// </summary>
        /// <param name="id">Primary key of the book to query.</param>
        public async Task<Book?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Ratings)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Returns books whose title, author name, or category name contains
        /// the given search term. Search is case-insensitive.
        /// </summary>
        /// <param name="query">Text to search for.</param>
        public async Task<IEnumerable<Book>> SearchAsync(string query)
        {
            query = query.ToLower();
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Title.ToLower().Contains(query) || 
                            b.Author.FullName.ToLower().Contains(query) || 
                            b.Category.Name.ToLower().Contains(query))
                .ToListAsync();
        }

        /// <summary>
        /// Returns books that have at least one copy available on the shelf.
        /// Used on lending screens to show only lend-able books.
        /// </summary>
        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _dbSet
                .Include(b => b.Author)
                .Where(b => b.AvailableCopies > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Returns the books that a specific user has added to their favorites,
        /// ordered by most recently added first.
        /// </summary>
        /// <param name="userId">ID of the user whose favorites are requested.</param>
        public async Task<IEnumerable<Book>> GetFavoritesAsync(int userId)
        {
            return await _context.UserFavorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Book).ThenInclude(b => b.Author)
                .Include(f => f.Book).ThenInclude(b => b.Category)
                .OrderByDescending(f => f.AddedDate)
                .Select(f => f.Book)
                .ToListAsync();
        }

        /// <summary>
        /// Checks whether the given user has already favorited the given book.
        /// </summary>
        public async Task<bool> IsFavoriteAsync(int userId, int bookId)
        {
            return await _context.UserFavorites
                .AnyAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        /// <summary>
        /// Toggles the favorite state: removes the record if it exists, adds it otherwise.
        /// </summary>
        /// <returns><c>true</c> = book was added to favorites; <c>false</c> = removed.</returns>
        public async Task<bool> ToggleFavoriteAsync(int userId, int bookId)
        {
            var existing = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

            if (existing != null)
            {
                _context.UserFavorites.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // removed
            }
            else
            {
                _context.UserFavorites.Add(new Models.UserFavorite { UserId = userId, BookId = bookId });
                await _context.SaveChangesAsync();
                return true;  // added
            }
        }

        /// <summary>
        /// Returns the most recently read books from a user's reading history.
        /// If the same book was read multiple times, only the most recent entry counts.
        /// Used for the "Recently Read" section on the profile page.
        /// </summary>
        /// <param name="userId">ID of the user whose history is queried.</param>
        /// <param name="count">Maximum number of books to return. Default: 20.</param>
        public async Task<IEnumerable<Book>> GetRecentlyReadAsync(int userId, int count = 20)
        {
            // Fetch unique book IDs ordered by the most recent read date
            var bookIds = await _context.ReadingHistories
                .Where(rh => rh.UserId == userId)
                .GroupBy(rh => rh.BookId)
                .Select(g => new { BookId = g.Key, LastRead = g.Max(x => x.LastReadDate) })
                .OrderByDescending(x => x.LastRead)
                .Take(count)
                .Select(x => x.BookId)
                .ToListAsync();

            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            // Preserve the original ordering from bookIds
            return bookIds.Select(id => books.First(b => b.Id == id));
        }

        /// <summary>
        /// Returns the most-read books based on reading history records.
        /// Falls back to books sorted by average rating when no reading history exists.
        /// Used for the statistics cards on the main screen.
        /// </summary>
        /// <param name="count">Maximum number of books to return. Default: 20.</param>
        public async Task<IEnumerable<Book>> GetMostReadAsync(int count = 20)
        {
            var bookIds = await _context.ReadingHistories
                .GroupBy(rh => rh.BookId)
                .Select(g => new { BookId = g.Key, ReadCount = g.Count() })
                .OrderByDescending(x => x.ReadCount)
                .Take(count)
                .Select(x => x.BookId)
                .ToListAsync();

            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            if (!bookIds.Any()) 
            {
                // No reading history yet — fall back to highest-rated books
                 return await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.AverageRating)
                    .Take(count)
                    .ToListAsync();
            }

            return bookIds.Select(id => books.First(b => b.Id == id));
        }
    }
}
