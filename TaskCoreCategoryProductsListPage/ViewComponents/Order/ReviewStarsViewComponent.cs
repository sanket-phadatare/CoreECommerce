

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.ViewComponents.Order
{
    public class ReviewStarsViewComponent : ViewComponent
    {
        private readonly ProductDbContext _context;

        public ReviewStarsViewComponent(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id, int productId)
        {
            //await Task.CompletedTask; // Just placeholder if nothing async

            ViewBag.OrderId = id;
            ViewBag.ProductId = productId;
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.OrderId == id && r.ProductId == productId);
            return View(review);
        }
    }
}
