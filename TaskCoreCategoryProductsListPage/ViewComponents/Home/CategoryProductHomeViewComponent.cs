using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.ViewComponents.Home
{
    public class CategoryProductHomeViewComponent : ViewComponent
    {
        private readonly ProductDbContext _context;
        public CategoryProductHomeViewComponent(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            return View(categories);
        }
    }
}
