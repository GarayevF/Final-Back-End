namespace Smartelectronics.Models
{
    public class Wishlist : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public double? Price { get; set; }
    }
}
