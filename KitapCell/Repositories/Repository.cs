using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KitapCell.Data;

namespace KitapCell.Repositories
{
    /// <summary>
    /// Generic base class for all entity repositories (Repository Pattern).
    /// Abstracts standard CRUD operations over Entity Framework Core.
    /// Entities that require custom queries inherit from this class and add extra methods.
    /// </summary>
    /// <typeparam name="T">The entity class that maps to a database table.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>EF Core database context instance.</summary>
        protected readonly LibraryDbContext _context;

        /// <summary>DbSet for the table managed by this repository.</summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes the repository with the provided DbContext.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(LibraryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>Finds a record by primary key. Returns <c>null</c> if not found.</summary>
        /// <param name="id">Primary key value.</param>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>Returns all records in the table.</summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Returns all records that satisfy the given condition.
        /// Supports flexible filtering via LINQ lambda expressions.
        /// </summary>
        /// <param name="predicate">Filter condition (e.g. <c>b => b.IsActive == true</c>).</param>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Adds a new record and persists the change to the database.
        /// </summary>
        /// <param name="entity">The entity object to insert.</param>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing record and persists the change to the database.
        /// </summary>
        /// <param name="entity">The entity object to update.</param>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the record with the specified ID from the database.
        /// Silently skips if the record is not found.
        /// </summary>
        /// <param name="id">Primary key of the record to delete.</param>
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
