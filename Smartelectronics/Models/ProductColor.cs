namespace Smartelectronics.Models
{
    public class ProductColor : BaseEntity
    {
        public string? Image { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? ColorId { get; set; }
        public Color? Color { get; set; }
    }
}
