using System.ComponentModel.DataAnnotations;

namespace Repositorys.DTOs.AccountDTO
{
    public class LoginDTO
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
