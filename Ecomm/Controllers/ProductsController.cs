using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecomm.Data;
using Ecomm.Models.ViewModels;

namespace Ecomm.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery, int? categoryId, string sortBy, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(p => 
                    p.Name.Contains(searchQuery) || 
                    p.Description.Contains(searchQuery));
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var viewModel = new ProductsViewModel
            {
                Products = await query.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                SearchQuery = searchQuery,
                CategoryId = categoryId,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductsGrid", viewModel);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
} 