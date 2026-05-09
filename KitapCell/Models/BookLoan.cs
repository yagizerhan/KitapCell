using System;

namespace KitapCell.Models
{
    public class BookLoan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        public LoanStatus Status { get; set; } = LoanStatus.Aktif;
        public decimal PenaltyAmount { get; set; } = 0;
        public string? Notes { get; set; }

        // Navigation Properties
        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
