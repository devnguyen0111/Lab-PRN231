using System.ComponentModel.DataAnnotations;

namespace Repositorys.DTOs.AccountDTO
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
