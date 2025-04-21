using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecomm.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetOrCreateSessionId()
        {
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
                _logger.LogInformation($"Created new session ID: {sessionId}");
            }
            else
            {
                _logger.LogInformation($"Using existing session ID: {sessionId}");
            }
            return sessionId;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = GetOrCreateSessionId();
            _logger.LogInformation($"Cart Index - Session ID: {sessionId}");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            _logger.LogInformation($"Found {cartItems.Count} items in cart");
            foreach (var item in cartItems)
            {
                _logger.LogInformation($"Cart item: ProductId={item.ProductId}, Quantity={item.Quantity}");
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var sessionId = GetOrCreateSessionId();
            _logger.LogInformation($"Adding to cart - Session ID: {sessionId}, Product ID: {productId}, Quantity: {quantity}");

            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product not found: {productId}");
                    return NotFound("Product not found");
                }

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        SessionId = sessionId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                    _logger.LogInformation($"Created new cart item for product {productId}");
                }
                else
                {
                    cartItem.Quantity += quantity;
                    _logger.LogInformation($"Updated quantity for product {productId} to {cartItem.Quantity}");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Changes saved to database");

                // Return JSON response for AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var totalItems = await GetCartItemsCount(sessionId);
                    return Json(new { success = true, cartCount = totalItems });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return StatusCode(500, "Error adding item to cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var sessionId = GetOrCreateSessionId();
            var count = await GetCartItemsCount(sessionId);
            _logger.LogInformation($"Cart count requested - Session ID: {sessionId}, Count: {count}");
            return Json(new { count });
        }

        private async Task<int> GetCartItemsCount(string sessionId)
        {
            return await _context.CartItems
                .Where(c => c.SessionId == sessionId)
                .SumAsync(c => c.Quantity);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var sessionId = GetOrCreateSessionId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.SessionId == sessionId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Removed item {id} from cart");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var totalItems = await GetCartItemsCount(sessionId);
                return Json(new { success = true, cartCount = totalItems });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var sessionId = GetOrCreateSessionId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.SessionId == sessionId);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated quantity for item {id} to {quantity}");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var totalItems = await GetCartItemsCount(sessionId);
                return Json(new { success = true, cartCount = totalItems });
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 