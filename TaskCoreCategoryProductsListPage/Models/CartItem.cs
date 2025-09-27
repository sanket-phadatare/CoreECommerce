using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskCoreCategoryProductsListPage.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int ProductId {  get; set; }
        public int Quantity { get; set; }
        public UserModel User { get; set; }
        public Product Product { get; set; }
    }
}
