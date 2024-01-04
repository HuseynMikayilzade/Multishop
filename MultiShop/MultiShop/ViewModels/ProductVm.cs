using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class ProductVm
    {
        public Product? Product { get; set; }
        public List<Product>? RelatedProducts { get; set; }
        public List<Category>? Categories { get; set; }

    }
}
