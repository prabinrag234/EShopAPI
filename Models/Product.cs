namespace EShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }                     // Primary key
        public string Name { get; set; }                // Product name
        public string Description { get; set; }         // Optional description
        public decimal Price { get; set; }              // Price in local currency
        public int StockQuantity { get; set; }          // Available units
        public int ShopId { get; set; }                 // Foreign key to Shop
        public Shop Shop { get; set; }                  // Navigation property

    }

}
