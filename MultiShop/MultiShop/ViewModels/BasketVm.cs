namespace MultiShop.ViewModels
{
    public class BasketVm
    {
        public ICollection<BasketItemVm>? basketItemVms { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal? AppliedCouponAmount { get; set; }
    }
}
