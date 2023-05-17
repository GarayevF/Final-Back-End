namespace Smartelectronics.Areas.Manage.ViewModels.ProductViewModels
{
    public class ColorVM
    {
        public int? ProductId { get; set; }
        public int? ColorId { get; set; }
        public IEnumerable<ProductColorImageVM>? ProductColorImageVMs { get; set; }
    }
}
