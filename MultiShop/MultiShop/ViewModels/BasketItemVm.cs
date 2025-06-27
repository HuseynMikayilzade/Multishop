using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class BasketItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public ICollection<int> ColorIds { get; set; }
        public ICollection<Color>? Colors { get; set; }
        public ICollection<int> SizeIds { get; set; }
        public ICollection<Size>? Sizes { get; set; }
    }
}
