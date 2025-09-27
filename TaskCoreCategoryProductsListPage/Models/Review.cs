namespace TaskCoreCategoryProductsListPage.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Sentiment { get; set; }
        public DateTime CreatedAt { get; set; }
        public Order Order { get; set; }
        
        public Product Product { get; set; }
    }
}
