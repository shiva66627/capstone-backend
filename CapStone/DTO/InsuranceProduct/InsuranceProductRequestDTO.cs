using System.ComponentModel.DataAnnotations;
using CapStone.Models;

namespace CapStone.DTO.InsuranceProduct
{
    public class InsuranceProductRequestDTO
    {
        [Required]
        [StringLength(100)]
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