using System.ComponentModel.DataAnnotations;

namespace Utils.DTOs.OrchidDTO
{
    public class CreateOrchidDTO
    {
        [Required]
        [StringLength(100)]
        public required string OrchidName { get; set; }

        [StringLength(255)]
        public string? OrchidDescription { get; set; }

        [Required]
        public required decimal Price { get; set; }

        public bool IsNatural { get; set; }

        [StringLength(255)]
        public string? OrchidUrl { get; set; }

        [Required]
        public required int CategoryId { get; set; }
    }
}