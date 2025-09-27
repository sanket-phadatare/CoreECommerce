namespace TaskCoreCategoryProductsListPage.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Tagline { get; set; }
        public IEnumerable<Product?> Products { get; set; }
    }
}
