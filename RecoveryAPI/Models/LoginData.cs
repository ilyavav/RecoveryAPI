using System.ComponentModel.DataAnnotations;

namespace RecoveryAPI.Models
{
    public class LoginData
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}