using System.ComponentModel.DataAnnotations;

namespace Repositorys.DTOs.AccountDTO
{
    public class ChangePasswordDTO
    {
        [Required]
        public required string CurrentPassword { get; set; }

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