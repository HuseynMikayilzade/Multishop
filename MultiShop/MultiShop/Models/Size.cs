namespace MultiShop.Models
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProductSize>? ProductSizes { get; set; }

    }
}
