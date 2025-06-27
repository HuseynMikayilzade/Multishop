namespace MultiShop.ViewModels
{
    public class CouponResultVm
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal Value { get; set; } 
        public int CouponId { get; set; }
    }
}
