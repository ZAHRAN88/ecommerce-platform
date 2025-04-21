using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
} 