using RecoveryAPI.Models;

namespace RecoveryAPI.Services
{
    public class AuthManager
    {
        private static readonly List<User> _users = CreateUsers();

        private static List<User> CreateUsers()
        {
            var user = new User
            {
                Login = "user",
                IsAdmin = false
            };
            user.SetPassword("user");

            var admin = new User
            {
                Login = "admin",
                IsAdmin = true
            };
            admin.SetPassword("admin");

            return new List<User> { user, admin };
        }

        public User? Authenticate(string login, string password)
        {
            var user = _users.FirstOrDefault(user => user.Login == login);

            if (user == null)
            {
                return null;
            }

            if (!user.CheckPassword(password))
            {
                return null;
            }

            return user;
        }
    }
}