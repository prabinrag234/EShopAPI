namespace EShopAPI.Models
{
    public class Shop
    {
        public int Id { get; set; }                     // Primary key
        public string Name { get; set; }                // Shop name
        public string OwnerId { get; set; }             // Keycloak user ID
        public string Location { get; set; }            // Optional address
        public List<Product> Products { get; set; }     // Navigation
    }

}
