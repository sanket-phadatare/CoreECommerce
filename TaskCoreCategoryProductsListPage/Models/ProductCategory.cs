using System.ComponentModel.DataAnnotations.Schema;

namespace TaskCoreCategoryProductsListPage.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId {  get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public Product Products { get; set; }
        public Category Categories { get; set; }
    }
}
