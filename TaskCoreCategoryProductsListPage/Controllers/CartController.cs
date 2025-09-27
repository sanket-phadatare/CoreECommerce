using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ProductDbContext _dbContext;
        public CartController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int? userId = Convert.ToInt32(Request.Cookies["userId"]);
            //List<CartItem> ci = _dbContext.CartItem.Where(c => c.UserId == userId).ToList();
            var cartItems = from ci in _dbContext.CartItem
                            join us in _dbContext.Users
                            on ci.UserId equals us.Id
                            where us.Id == userId
                            join p in _dbContext.Products
                            on ci.ProductId equals p.ProductId
                            select new { ProductName = p.Name, ProductId = ci.ProductId, Quantity = ci.Quantity, Price = p.Price, StockQuantity = p.StockQuantity };
            
            return View(cartItems);
        }
        [HttpPost]
        public void AddItems([FromBody] CartItem cartItem)
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            cartItem.UserId = userId;

            if (_dbContext.CartItem.Any(c => c.ProductId == cartItem.ProductId && c.UserId == cartItem.UserId))
            {
                CartItem ci = _dbContext.CartItem.FirstOrDefault(ci => ci.ProductId == cartItem.ProductId && ci.UserId == cartItem.UserId);

                if (cartItem.Quantity == 0)
                {
                    _dbContext.Remove(ci);
                    _dbContext.SaveChanges();
                }
                else
                {
                    ci.Quantity = cartItem.Quantity;
                    _dbContext.CartItem.Entry(ci).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                
            }
            else
            {
                _dbContext.CartItem.Add(cartItem);
                _dbContext.SaveChanges();
            }
            
        }
        [HttpGet]
        [AllowAnonymous]
        public int CartItemCount()
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            int cartItemCount = _dbContext.CartItem.Where(c => c.UserId == userId).Count();
            return cartItemCount;
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int productId)
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            CartItem ci = _dbContext.CartItem.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);
            _dbContext.Remove(ci);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ClearCart()
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            List<CartItem> ci = _dbContext.CartItem.Where(c => c.UserId == userId).ToList();
            _dbContext.RemoveRange(ci);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            CartItem ci = new CartItem()
            {
                ProductId = productId,
                Quantity = quantity
            };
            AddItems(ci);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> BuyNow()
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            var cartItems = await _dbContext.CartItem
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // 🔹 Check stock availability
            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product.StockQuantity)
                {
                    TempData["Error"] = $"Not enough stock for product: {item.Product.Name}. " +
                                        $"Available: {item.Product.StockQuantity}, Requested: {item.Quantity}";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // 🔹 If all good, process order inside transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderItems = cartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                _dbContext.Orders.Add(order);

                // Reduce stock
                foreach (var item in cartItems)
                {
                    item.Product.StockQuantity -= item.Quantity;
                }

                // Clear cart
                _dbContext.CartItem.RemoveRange(cartItems);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Checkout failed: {ex.Message}";
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}
