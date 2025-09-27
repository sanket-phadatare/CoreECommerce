using System.ComponentModel.DataAnnotations.Schema;

namespace TaskCoreCategoryProductsListPage.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public UserModel User { get; set; }
    }
}
