using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ProductDbContext _dbContext;
        public OrderController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int userId = Convert.ToInt32(Request.Cookies["userId"]);
            List<Order> orders = _dbContext.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Where(o => o.UserId == userId).OrderByDescending(od => od.OrderDate).ToList();
            
            return View(orders);
        }
    }
}
