namespace Smartelectronics.Areas.Manage.ViewModels.ProductViewModels
{
    public class ColorImageVM
    {
        public int ColorId { get; set; }
        public string? Name { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
