using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class AddToCartRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
} 