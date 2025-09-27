using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDbContext _context;

        public ProductController(ProductDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryList = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product,int CatId)
        {
            if (ModelState.IsValid)
            {
                
                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();

                Product addedProduct = _context.Products.Where(m => m.Name == product.Name).First();

                ProductCategory productCategory = new ProductCategory()
                {
                    ProductId = product.ProductId,
                    CategoryId = CatId
                };
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products.FindAsync(id);
            IEnumerable<Category> category = from c in _context.Categories
                                join pc in _context.ProductCategories
                                on c.CategoryId equals pc.CategoryId
                                where pc.ProductId == id
                                select c;
            ViewBag.CategoryId = category.FirstOrDefault().CategoryId;
            ViewBag.CategoryList = _context.Categories.ToList();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,StockQuantity,CreatedAt,UpdatedAt")] Product product, int CatId)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    ProductCategory pc = _context.ProductCategories.FirstOrDefault(p => p.ProductId == id);
                    pc.CategoryId = CatId;
                    _context.ProductCategories.Entry(pc).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ProductListing(int? id)
        {
            ViewBag.CategoryName = _context.Categories?.FirstOrDefault(c => c.CategoryId == id)?.CategoryName?.ToString() ?? null;
            List<Product> products = new List<Product>();
            if (id != null || id == 0)
            {
                products = await (from p in _context.Products
                                                join pc in _context.ProductCategories
                                                on p.ProductId equals pc.ProductId
                                                join c in _context.Categories
                                                on pc.CategoryId equals c.CategoryId
                                                where pc.CategoryId == id
                                                select p).ToListAsync();
            }
            else
            {
                products = _context.Products.ToList();
            }


                int userId = Convert.ToInt32(Request.Cookies["userId"]);

            ViewBag.CartItems = _context.CartItem.Where(ci => ci.UserId == userId).ToList();
            return View(products);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
