using Microsoft.AspNetCore.Identity;

namespace RecoveryAPI.Models

{
    public class User
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public void SetPassword(string password)
        {
            PasswordHash = _passwordHasher.HashPassword(this, password);
        }

        public bool CheckPassword(string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(this, PasswordHash, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}