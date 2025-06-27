namespace MultiShop.Models
{
    public class CouponUsage
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public Coupon? Coupon { get; set; }
        public string UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? IpAddress { get; set; }
    }
}
