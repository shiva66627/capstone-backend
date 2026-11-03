using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.InsuranceProduct
{
    public class InsuranceProductRequestDTO
    {
        [Required]
        [StringLength(100)]
        [MinLength(10, ErrorMessage = "Product name must be at least 10 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public ProductType ProductType { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ActiveStatus Status { get; set; }
    }
}