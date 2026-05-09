using System;
using System.Linq;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Models;
using KitapCell.Services;
using Microsoft.EntityFrameworkCore;
using KitapCell.Core;

namespace KitapCell
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            // Load application settings from disk before the UI starts
            SettingsManager.Load();

            // Initialize the database: apply migrations, then seed default data.
            // This is the single authoritative place for DB startup.
            try
            {
                using var context = new LibraryDbContext();

                // Apply (or create) all pending EF Core migrations.
                // Migrate() is safe to call on an empty DB — it creates the schema.
                context.Database.Migrate();

                // Seed default data that may not be covered by migrations.
                SeedDatabase(context);
            }
            catch (Exception ex)
            {
                // A database failure at startup is fatal — the app cannot run
                // without its schema. Show a user-friendly error and exit.
                MessageBox.Show(
                    $"Veritabanı başlatılamadı. Uygulama kapatılacak.\n\nDetay: {ex.Message}",
                    "Kritik Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm());
        }

        /// <summary>
        /// Ensures that the mandatory seed records (admin user, default categories)
        /// always exist after migration, regardless of whether they were part of a
        /// HasData() migration. This is the safest approach for self-contained deploys.
        /// </summary>
        private static void SeedDatabase(LibraryDbContext context)
        {
            // ── Default admin user ───────────────────────────────────────────────
            if (!context.Users.Any(u => u.Email == "admin@library.com"))
            {
                context.Users.Add(new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    IdentityNumber = "11111111111",
                    Email = "admin@library.com",
                    PasswordHash = PasswordHelper.Hash("123"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    CanAddBook = true,
                    CanEditBook = true,
                    CanDeleteBook = true,
                    RegistrationDate = new DateTime(2024, 1, 1)
                });
            }

            // ── Default categories ───────────────────────────────────────────────
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Genel",             IconEmoji = "📚" },
                    new Category { Name = "Roman",             IconEmoji = "📖" },
                    new Category { Name = "Bilim Kurgu",       IconEmoji = "🚀" },
                    new Category { Name = "Tarih",             IconEmoji = "🏛️" },
                    new Category { Name = "Bilim",             IconEmoji = "🔬" }
                );
            }

            context.SaveChanges();
        }
    }
}