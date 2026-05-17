using Microsoft.EntityFrameworkCore;
using KitapCell.Models;

namespace KitapCell.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<BookLoan> BookLoans { get; set; } = null!;
        public DbSet<UserRating> UserRatings { get; set; } = null!;
        public DbSet<UserFavorite> UserFavorites { get; set; } = null!;
        public DbSet<ReadingHistory> ReadingHistories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use AppData folder to avoid permission issues when installed in Program Files
                string appDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KitapCell");
                if (!System.IO.Directory.Exists(appDataFolder))
                {
                    System.IO.Directory.CreateDirectory(appDataFolder);
                }
                string dbPath = System.IO.Path.Combine(appDataFolder, "library.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Relationships ────────────────────────────────────────────────────

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookLoan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookLoan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRating>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavorite>()
                .HasOne(f => f.Book)
                .WithMany(b => b.Favorites)
                .HasForeignKey(f => f.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.User)
                .WithMany(u => u.ReadingHistories)
                .HasForeignKey(rh => rh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Book)
                .WithMany(b => b.ReadingHistories)
                .HasForeignKey(rh => rh.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Unique Constraints ───────────────────────────────────────────────

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // ── Seed Data ────────────────────────────────────────────────────────

            // Default admin user — ensures a known admin account (id=1) exists on
            // first run so that relational tables don't fail on empty foreign keys.
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Kullanıcı",
                IdentityNumber = "11111111111",
                Email = "admin@library.com",
                PasswordHash = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3", // plain: 123
                Role = UserRole.Admin,
                IsActive = true,
                CanAddBook = true,
                CanEditBook = true,
                CanDeleteBook = true,
                ReputationScore = 0,
                RegistrationDate = new DateTime(2024, 1, 1)
            });

            // Default categories — seeded to prevent SQLite foreign-key error 19
            // when books reference a category that doesn't exist yet.
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Genel",        IconEmoji = "📚" },
                new Category { Id = 2, Name = "Roman",        IconEmoji = "📖" },
                new Category { Id = 3, Name = "Bilim Kurgu",  IconEmoji = "🚀" },
                new Category { Id = 4, Name = "Tarih",        IconEmoji = "🏛️" },
                new Category { Id = 5, Name = "Bilim",        IconEmoji = "🔬" }
            );
        }
    }
}
