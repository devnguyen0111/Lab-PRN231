using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.CategoryDTO
{
    public class UpdateCategoryDTO
    {
        [Required]
        [StringLength(50)]
        public required string CategoryName { get; set; }
    }
}