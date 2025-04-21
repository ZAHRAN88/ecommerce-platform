using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string SessionId { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
} 