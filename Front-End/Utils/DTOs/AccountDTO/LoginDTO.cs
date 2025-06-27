using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.AccountDTO
{
    public class LoginDTO
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
