namespace Smartelectronics.Models
{
    public class Compare : BaseEntity
    {
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public double? Price { get; set; }
    }
}
