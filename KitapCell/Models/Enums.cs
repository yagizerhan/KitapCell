namespace KitapCell.Models
{
    /// <summary>
    /// Defines the possible roles that a user account can hold in the system.
    /// Roles control which screens and administrative actions are accessible.
    /// </summary>
    public enum UserRole
    {
        /// <summary>Standard library member. Can browse, borrow, rate books, and use the web reader.</summary>
        Uye = 0,

        /// <summary>System administrator. Full access to all settings, users, and reports.</summary>
        Admin = 2
    }

    /// <summary>
    /// Tracks the lifecycle of a <see cref="BookLoan"/> record.
    /// </summary>
    public enum LoanStatus
    {
        /// <summary>Book is currently out on loan and the due date has not been reached.</summary>
        Aktif = 0,

        /// <summary>Book was returned on or before the due date.</summary>
        IadeEdildi = 1,

        /// <summary>Book was returned after the due date; a late-return penalty was applied.</summary>
        Gecikti = 2
    }
}
