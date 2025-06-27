using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class ShopVm
    {
        public List<Product>? Products { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPage { get; set; }
        public string? CategoryName { get; set; }
    }
}
