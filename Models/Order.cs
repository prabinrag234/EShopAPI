namespace EShopAPI.Models
{
    public class Order
    {
        public int Id { get; set; }                     // Primary key
        public DateTime OrderDate { get; set; }         // Timestamp
        public string CustomerId { get; set; }          // Keycloak user ID
        public List<OrderItem> Items { get; set; }      // List of products
        public decimal TotalAmount { get; set; }        // Calculated total
    }

    public class OrderItem
    {
        public int Id { get; set; }                     // Primary key
        public int ProductId { get; set; }              // Foreign key
        public Product Product { get; set; }            // Navigation
        public int Quantity { get; set; }               // Units ordered
        public decimal UnitPrice { get; set; }          // Price at time of order
    }

}
