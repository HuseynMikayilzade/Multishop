namespace MultiShop.Models
{
    public class ProductColor
    {
        public int Id { get; set; }

        public int PorductId { get; set; }
        public Product? Product { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }

    }
}
