using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.AccountDTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public required string ConfirmNewPassword { get; set; }
    }
}
