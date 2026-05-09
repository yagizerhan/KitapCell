using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KitapCell.Models;
using KitapCell.Data;

namespace KitapCell.Repositories
{
    /// <summary>
    /// Handles database operations specific to User records.
    /// Inherits basic CRUD from <see cref="Repository{T}"/>.
    /// </summary>
    public class UserRepository : Repository<User>
    {
        public UserRepository(LibraryDbContext context) : base(context) { }

        /// <summary>
        /// Looks up a user by email address. Returns <c>null</c> if not found.
        /// Used during login authentication.
        /// </summary>
        /// <param name="email">Email address to search for.</param>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Returns the user with the specified ID together with their loan history,
        /// favorites, ratings, and reading history (all eagerly loaded).
        /// Used on screens that need the full user profile, such as the profile page.
        /// </summary>
        /// <param name="id">Primary key of the user to query.</param>
        public async Task<User?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Loans).ThenInclude(l => l.Book)
                .Include(u => u.Favorites).ThenInclude(f => f.Book)
                .Include(u => u.Ratings).ThenInclude(r => r.Book)
                .Include(u => u.ReadingHistories).ThenInclude(rh => rh.Book)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Returns all users sorted first by role, then by last name,
        /// with their loan records eagerly loaded.
        /// Used to display the member list on admin screens.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllWithLoansAsync()
        {
            return await _dbSet
                .Include(u => u.Loans)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }
    }
}
