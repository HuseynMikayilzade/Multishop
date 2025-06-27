namespace MultiShop.ViewModels
{
    public class CalculateTotalVm
    {
        public ICollection<BasketItemVm>? Items { get; set; }
        public double? CouponDiscount { get; set; }
    }
}
