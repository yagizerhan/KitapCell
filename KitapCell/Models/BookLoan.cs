using System;

namespace KitapCell.Models
{
    /// <summary>
    /// Records a single loan transaction — one user borrowing one book.
    /// A new record is created when a book is lent, and updated on return.
    /// The <see cref="Status"/> field tracks the lifecycle of the transaction.
    /// </summary>
    public class BookLoan
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Foreign key referencing the borrowed <see cref="Book"/>.</summary>
        public int BookId { get; set; }

        /// <summary>Foreign key referencing the borrowing <see cref="User"/>.</summary>
        public int UserId { get; set; }

        /// <summary>Date and time the book was physically lent out.</summary>
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The deadline by which the book must be returned.
        /// Set to <see cref="BorrowDate"/> + configured loan days at the time of lending.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date and time the book was actually returned.
        /// Null while the loan is still active.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Current state of the loan.
        /// <see cref="LoanStatus.Aktif"/>: book is out on loan.
        /// <see cref="LoanStatus.IadeEdildi"/>: book returned on time.
        /// <see cref="LoanStatus.Gecikti"/>: returned late (penalty applied).
        /// </summary>
        public LoanStatus Status { get; set; } = LoanStatus.Aktif;

        /// <summary>
        /// Late-return fine in Turkish Lira (TL).
        /// Calculated as: overdue days × 5 TL per day.
        /// Zero for on-time returns.
        /// </summary>
        public decimal PenaltyAmount { get; set; } = 0;

        /// <summary>Optional notes added by the librarian at the time of lending or return.</summary>
        public string? Notes { get; set; }

        // Navigation Properties
        /// <summary>The book that was borrowed.</summary>
        public Book Book { get; set; } = null!;

        /// <summary>The user who borrowed the book.</summary>
        public User User { get; set; } = null!;
    }
}
