namespace MultiShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public string?  Status { get; set; }
        public decimal? CouponDiscount { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public DateTime Received { get; set; }
    }
}
