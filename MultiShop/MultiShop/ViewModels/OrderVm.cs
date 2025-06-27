using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class OrderVm
    {
        public string Adress { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CouponDiscount { get; set; } = 0;
        public decimal Total { get; set; }
        public List<BasketItem>? BasketItems{ get; set; }
    }
}
