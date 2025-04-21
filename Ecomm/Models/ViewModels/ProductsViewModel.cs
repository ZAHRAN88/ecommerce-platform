using System.Collections.Generic;

namespace Ecomm.Models.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public string? SearchQuery { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
} 