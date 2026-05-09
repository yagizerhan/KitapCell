using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KitapCell.Models;
using KitapCell.Data;

namespace KitapCell.Repositories
{
    /// <summary>
    /// Handles database operations specific to BookLoan records.
    /// Inherits basic CRUD from <see cref="Repository{T}"/>;
    /// this class only contains loan-specific queries.
    /// </summary>
    public class LoanRepository : Repository<BookLoan>
    {
        public LoanRepository(LibraryDbContext context) : base(context) { }

        /// <summary>
        /// Returns all active loan records — those with status
        /// <see cref="LoanStatus.Aktif"/> or <see cref="LoanStatus.Gecikti"/> —
        /// with related book and user data eagerly loaded.
        /// </summary>
        public async Task<IEnumerable<BookLoan>> GetActiveLoansAsync()
        {
            return await _dbSet
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Aktif || l.Status == LoanStatus.Gecikti)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all loan records ordered by borrow date descending,
        /// with related book and user data eagerly loaded.
        /// </summary>
        public async Task<IEnumerable<BookLoan>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(l => l.Book)
                .Include(l => l.User)
                .OrderByDescending(l => l.BorrowDate)
                .ToListAsync();
        }

        /// <summary>
        /// Returns loan records with status <see cref="LoanStatus.Gecikti"/>
        /// — i.e. past their due date and not yet returned.
        /// </summary>
        public async Task<IEnumerable<BookLoan>> GetOverdueLoansAsync()
        {
            return await _dbSet
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Gecikti)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all loan records belonging to a specific user,
        /// ordered by borrow date descending.
        /// </summary>
        /// <param name="userId">ID of the user whose loan history is requested.</param>
        public async Task<IEnumerable<BookLoan>> GetLoansByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(l => l.Book)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.BorrowDate)
                .ToListAsync();
        }
    }
}
