using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecomm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products based on category and similar price range
            var relatedProducts = await _context.Products
                .Where(p => p.Id != id && 
                           (p.CategoryId == product.CategoryId || 
                            (p.Price >= product.Price * 0.8m && p.Price <= product.Price * 1.2m)))
                .OrderByDescending(p => p.CategoryId == product.CategoryId)
                .ThenBy(p => Math.Abs((double)(p.Price - product.Price)))
                .Take(8)
                .ToListAsync();

            ViewData["RelatedProducts"] = relatedProducts;
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
