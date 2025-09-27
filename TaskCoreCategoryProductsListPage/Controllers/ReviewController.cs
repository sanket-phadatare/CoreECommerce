using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ProductDbContext _context;
        public ReviewController(ProductDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "Poor");
            dict.Add(2, "Bad");
            dict.Add(3, "Average");
            dict.Add(4, "Good");
            dict.Add(5, "Best");
            review.Sentiment = dict[review.Rating];
            Review existingReview = await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(r => review.OrderId == r.OrderId && review.ProductId == r.ProductId);
            if (existingReview != null)
            {
                
                return await Update(review); 
            }
            else
            {
                review.CreatedAt = DateTime.Now;
                await _context.AddAsync(review);
                await _context.SaveChangesAsync();

                return Ok();
            }

        }

        
        public async Task<IActionResult> Update(Review review)
        {
            Review existingReview = await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(r => review.OrderId == r.OrderId && review.ProductId == r.ProductId);
            if (existingReview != null)
            {
                review.CreatedAt = existingReview.CreatedAt;
            review.ReviewId = existingReview.ReviewId;
                _context.Reviews.Update(review);
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(int orderId, int productId)
        {
            Review review = await _context.Reviews.FirstOrDefaultAsync(r => orderId == r.OrderId && productId == r.ProductId);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Order");
        }
    }
}
