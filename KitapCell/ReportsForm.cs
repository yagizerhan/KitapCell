using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KitapCell.Data;
using KitapCell.Core;
using Microsoft.EntityFrameworkCore;

namespace KitapCell
{
    public class ReportsForm : Form
    {
        private LibraryDbContext _db;
        private Panel pnlMain;
        private Label lblTitle;
        private FlowLayoutPanel flpCards;
        private Panel pnlTopBooks;
        private Panel pnlTopUsers;
        private Label lblLoading;

        public ReportsForm()
        {
            _db = new LibraryDbContext();
            BuildUI();
            ThemeHelper.Apply(this);
        }

        private void BuildUI()
        {
            this.Text = "📊 Kütüphane Raporları";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.MinimumSize = new Size(750, 550);

            lblTitle = new Label
            {
                Text = "📊  Kütüphane İstatistikleri",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };

            lblLoading = new Label
            {
                Text = "⏳  Veriler yükleniyor...",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(139, 148, 158),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 16, 16),
                AutoScroll = true
            };

            // Özet kartlar (top row)
            flpCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 8)
            };

            // En çok ödünç alınan kitaplar
            pnlTopBooks = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(16),
                Margin = new Padding(0, 12, 0, 0)
            };

            // En aktif üyeler
            pnlTopUsers = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                BackColor = Color.FromArgb(22, 27, 34),
                Padding = new Padding(16),
                Margin = new Padding(0, 12, 0, 0)
            };

            pnlMain.Controls.Add(lblLoading);
            this.Controls.Add(pnlMain);
            this.Controls.Add(lblTitle);

            this.Load += async (s, e) => await LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                // ── Temel sayılar ──────────────────────────────────
                int totalBooks    = await _db.Books.CountAsync();
                int totalUsers    = await _db.Users.CountAsync(u => !u.Role.Equals(Models.UserRole.Admin));
                int activeLoans   = await _db.BookLoans.CountAsync(l => l.ReturnDate == null);
                int totalLoans    = await _db.BookLoans.CountAsync();
                int digitalBooks  = await _db.Books.CountAsync(b => b.HasDigitalCopy);
                int overdueLoans  = await _db.BookLoans.CountAsync(l => l.ReturnDate == null && l.DueDate < DateTime.Now);
                int totalFavs     = await _db.UserFavorites.CountAsync();
                int totalReadSessions = await _db.ReadingHistories.CountAsync();

                // ── En çok ödünç alınan 5 kitap ───────────────────
                var topBooks = await _db.BookLoans
                    .GroupBy(l => l.BookId)
                    .Select(g => new { BookId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Join(_db.Books, x => x.BookId, b => b.Id, (x, b) => new { b.Title, x.Count })
                    .ToListAsync();

                // ── En aktif 5 üye ─────────────────────────────────
                var topUsers = await _db.BookLoans
                    .GroupBy(l => l.UserId)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Join(_db.Users, x => x.UserId, u => u.Id,
                        (x, u) => new { Name = u.FirstName + " " + u.LastName, x.Count })
                    .ToListAsync();

                // ── Kategori dağılımı ──────────────────────────────
                var catStats = await _db.Books
                    .GroupBy(b => b.CategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Join(_db.Categories, x => x.CategoryId, c => c.Id,
                        (x, c) => new { c.Name, c.IconEmoji, x.Count })
                    .ToListAsync();

                // ── UI'ya aktar (UI thread) ────────────────────────
                if (IsDisposed) return;
                this.Invoke(() =>
                {
                    lblLoading.Visible = false;
                    pnlMain.Controls.Remove(lblLoading);

                    // --- Özet Kartlar ---
                    var cardDefs = new[]
                    {
                        ("📚", "Toplam Kitap",    totalBooks.ToString(),      Color.FromArgb(99, 102, 241)),
                        ("👥", "Üye Sayısı",      totalUsers.ToString(),      Color.FromArgb(59, 130, 246)),
                        ("📤", "Aktif Ödünç",     activeLoans.ToString(),     Color.FromArgb(34, 197, 94)),
                        ("⚠️", "Gecikmiş",        overdueLoans.ToString(),    Color.FromArgb(239, 68, 68)),
                        ("💻", "Dijital Kitap",   digitalBooks.ToString(),    Color.FromArgb(34, 211, 238)),
                        ("❤️", "Toplam Favori",   totalFavs.ToString(),       Color.FromArgb(245, 158, 11)),
                        ("📖", "Okuma Kaydı",     totalReadSessions.ToString(),Color.FromArgb(168, 85, 247)),
                        ("📋", "Toplam Ödünç",    totalLoans.ToString(),      Color.FromArgb(20, 184, 166)),
                    };

                    foreach (var (icon, label, value, color) in cardDefs)
                        flpCards.Controls.Add(MakeStatCard(icon, label, value, color));

                    pnlMain.Controls.Add(pnlTopUsers);
                    pnlMain.Controls.Add(pnlTopBooks);
                    pnlMain.Controls.Add(flpCards);

                    // En çok ödünç alınan kitaplar
                    BuildTopPanel(pnlTopBooks,
                        "📤  En Çok Ödünç Alınan Kitaplar",
                        topBooks.Select(x => (x.Title, x.Count)).ToList(),
                        Color.FromArgb(34, 197, 94));

                    // En aktif üyeler
                    BuildTopPanel(pnlTopUsers,
                        "🏆  En Aktif Üyeler",
                        topUsers.Select(x => (x.Name, x.Count)).ToList(),
                        Color.FromArgb(99, 102, 241));
                });
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    this.Invoke(() => MessageBox.Show("Rapor yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private Panel MakeStatCard(string icon, string label, string value, Color accent)
        {
            var card = new Panel
            {
                Width = 155,
                Height = 110,
                BackColor = Color.FromArgb(22, 27, 34),
                Margin = new Padding(0, 0, 12, 0)
            };

            // Sol şerit
            var stripe = new Panel { BackColor = accent, Width = 4, Dock = DockStyle.Left };

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = accent,
                Location = new Point(14, 10),
                Size = new Size(44, 44),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblVal = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(14, 52),
                Size = new Size(130, 32),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblLbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(110, 118, 138),
                Location = new Point(14, 83),
                Size = new Size(130, 18),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.AddRange(new Control[] { stripe, lblIcon, lblVal, lblLbl });

            // Hover
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(31, 36, 44);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(22, 27, 34);

            return card;
        }

        private void BuildTopPanel(Panel pnl, string title, List<(string Name, int Count)> items, Color accent)
        {
            pnl.Controls.Clear();
            pnl.BackColor = Color.FromArgb(22, 27, 34);

            var lblH = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnl.Controls.Add(lblH);

            if (items.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Henüz veri yok.",
                    ForeColor = Color.FromArgb(139, 148, 158),
                    Font = new Font("Segoe UI", 11F),
                    Dock = DockStyle.Top,
                    Height = 30,
                    Padding = new Padding(4, 4, 0, 0)
                };
                pnl.Controls.Add(lblEmpty);
                return;
            }

            int maxCount = items.Max(x => x.Count);

            for (int i = 0; i < items.Count; i++)
            {
                var (name, count) = items[i];
                var row = BuildBarRow(i + 1, name, count, maxCount, accent);
                pnl.Controls.Add(row);
            }
        }

        private Panel BuildBarRow(int rank, string name, int count, int maxCount, Color accent)
        {
            var row = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.Transparent };

            var lblRank = new Label
            {
                Text = $"#{rank}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 102, 241),
                Location = new Point(0, 10),
                Size = new Size(28, 18)
            };

            var lblName = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(201, 209, 217),
                Location = new Point(30, 4),
                Size = new Size(340, 18),
                AutoEllipsis = true
            };

            var lblCount = new Label
            {
                Text = $"{count} kez",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(30, 20),
                Size = new Size(100, 14)
            };

            // Progress bar
            int barMaxW = 340;
            int barW = maxCount > 0 ? (int)((double)count / maxCount * barMaxW) : 0;

            var barBg = new Panel
            {
                BackColor = Color.FromArgb(35, 40, 58),
                Location = new Point(390, 15),
                Size = new Size(barMaxW, 8)
            };
            var barFill = new Panel
            {
                BackColor = accent,
                Location = new Point(0, 0),
                Size = new Size(barW, 8)
            };
            barBg.Controls.Add(barFill);

            row.Controls.AddRange(new Control[] { lblRank, lblName, lblCount, barBg });
            return row;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _db?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
