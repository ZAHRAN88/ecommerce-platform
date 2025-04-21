using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecomm.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ApplicationDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetOrCreateSessionId()
        {
            var sessionId = HttpContext.Session.GetString("CartSessionId"); // Use same session as cart
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
                _logger.LogInformation($"Created new session ID: {sessionId}");
            }
            return sessionId;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = GetOrCreateSessionId();
            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.SessionId == sessionId)
                .OrderByDescending(w => w.DateAdded)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var sessionId = GetOrCreateSessionId();
            _logger.LogInformation($"Adding to wishlist - Session ID: {sessionId}, Product ID: {productId}");

            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product not found: {productId}");
                    return NotFound("Product not found");
                }

                var existingItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(w => w.SessionId == sessionId && w.ProductId == productId);

                if (existingItem == null)
                {
                    var wishlistItem = new WishlistItem
                    {
                        SessionId = sessionId,
                        ProductId = productId,
                        DateAdded = DateTime.UtcNow
                    };
                    _context.WishlistItems.Add(wishlistItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Added product {productId} to wishlist");
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var count = await GetWishlistCount(sessionId);
                    return Json(new { success = true, wishlistCount = count });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to wishlist");
                return StatusCode(500, "Error adding item to wishlist");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            var sessionId = GetOrCreateSessionId();
            var count = await GetWishlistCount(sessionId);
            return Json(new { count });
        }

        private async Task<int> GetWishlistCount(string sessionId)
        {
            return await _context.WishlistItems
                .Where(w => w.SessionId == sessionId)
                .CountAsync();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var sessionId = GetOrCreateSessionId();
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.SessionId == sessionId);

            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Removed item {id} from wishlist");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var count = await GetWishlistCount(sessionId);
                return Json(new { success = true, wishlistCount = count });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MoveToCart(int id)
        {
            var sessionId = GetOrCreateSessionId();
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.SessionId == sessionId);

            if (wishlistItem != null)
            {
                // Add to cart
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == wishlistItem.ProductId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        SessionId = sessionId,
                        ProductId = wishlistItem.ProductId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += 1;
                }

                // Remove from wishlist
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Moved item {id} from wishlist to cart");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var wishlistCount = await GetWishlistCount(sessionId);
                var cartCount = await _context.CartItems
                    .Where(c => c.SessionId == sessionId)
                    .SumAsync(c => c.Quantity);
                return Json(new { success = true, wishlistCount, cartCount });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearWishlist()
        {
            var sessionId = GetOrCreateSessionId();
            
            try
            {
                var wishlistItems = await _context.WishlistItems
                    .Where(w => w.SessionId == sessionId)
                    .ToListAsync();

                if (wishlistItems.Any())
                {
                    _context.WishlistItems.RemoveRange(wishlistItems);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Cleared all items from wishlist for session {sessionId}");
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing wishlist");
                return StatusCode(500, "Error clearing wishlist");
            }
        }
    }
} 