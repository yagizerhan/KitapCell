using System;
using System.Threading.Tasks;
using KitapCell.Models;
using KitapCell.Repositories;

namespace KitapCell.Services
{
    /// <summary>
    /// Contains business logic for user account management.
    /// Handles local login/registration for the desktop application and
    /// manages reputation score updates.
    /// Web interface authentication is handled separately in <c>ApiEndpoints.cs</c>
    /// using BCrypt directly.
    /// </summary>
    public class UserService
    {
        private readonly UserRepository _userRepository;

        /// <summary>Receives the UserRepository via dependency injection.</summary>
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticates a user by email and password for the desktop application.
        /// Returns <c>null</c> if the user is not found or the account is inactive.
        /// </summary>
        /// <param name="email">The email address of the user attempting to log in.</param>
        /// <param name="password">The plain-text password to verify.</param>
        /// <returns>The <see cref="User"/> object on success, or <c>null</c> on failure.</returns>
        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
                return null;

            // Verify the supplied password against the stored BCrypt hash
            if (Services.PasswordHelper.Verify(password, user.PasswordHash))
                return user;

            return null;
        }

        /// <summary>
        /// Registers a new user account. Registration is rejected if the email is already taken.
        /// The password is hashed with BCrypt before being written to the database — plain-text is never stored.
        /// </summary>
        /// <param name="user">The user object to register (PasswordHash must be empty).</param>
        /// <param name="plainPassword">The plain-text password that will be hashed.</param>
        /// <returns><c>true</c> if registration succeeded; <c>false</c> if the email is already in use.</returns>
        public async Task<bool> RegisterAsync(User user, string plainPassword)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                return false; // Email already registered

            // Hash the password; plain-text is never stored in the database
            user.PasswordHash = PasswordHelper.Hash(plainPassword);

            await _userRepository.AddAsync(user);
            return true;
        }

        /// <summary>
        /// Adjusts the user's <see cref="User.ReputationScore"/> by the specified delta.
        /// The score cannot drop below zero.
        /// Typical deltas: on-time return: +5 | late return: -10 | writing a review: +10.
        /// </summary>
        /// <param name="userId">ID of the user whose score will be updated.</param>
        /// <param name="delta">Amount to add (or subtract if negative).</param>
        public async Task UpdateReputationAsync(int userId, int delta)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.ReputationScore += delta;
                if (user.ReputationScore < 0) user.ReputationScore = 0; // Clamp to zero

                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
