using System;
using System.Collections.Generic;

namespace KitapCell.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        
        public string? Publisher { get; set; }
        public int? PublishYear { get; set; }
        
        public int TotalCopies { get; set; } = 1;
        public int AvailableCopies { get; set; } = 1;
        
        public string? Description { get; set; }
        public string Language { get; set; } = "Türkçe";
        public int? PageCount { get; set; }
        
        public string? CoverImagePath { get; set; }
        public string? PdfFilePath { get; set; }
        public bool HasDigitalCopy { get; set; } = false;
        
        public float AverageRating { get; set; } = 0;
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string? Location { get; set; }

        // Navigation Properties
        public Author Author { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<BookLoan> Loans { get; set; } = new List<BookLoan>();
        public ICollection<UserRating> Ratings { get; set; } = new List<UserRating>();
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
    }
}
