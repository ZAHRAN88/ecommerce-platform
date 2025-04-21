using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        
        [Required]
        public string SessionId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public Product Product { get; set; }
        
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
} 