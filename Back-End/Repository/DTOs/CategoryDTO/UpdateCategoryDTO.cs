using System.ComponentModel.DataAnnotations;

namespace Repositorys.DTOs.CategoryDTO
{
    public class UpdateCategoryDTO
    {
        [Required]
        [StringLength(50)]
        public required string CategoryName { get; set; }
    }
}