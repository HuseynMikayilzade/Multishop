namespace MultiShop.ViewModels
{
    public class WishListItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }
        public string? Description { get; set; }
    }
}
