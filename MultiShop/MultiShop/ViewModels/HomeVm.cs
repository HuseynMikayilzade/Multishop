using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class HomeVm
    {
        public List<Slide>? Slides { get; set; }
        public List<SpecialProduct>? SpecialProducts { get; set; }
        public List<CustomService>? CustomServices { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Product>? Products { get; set; }
     
    }
}
