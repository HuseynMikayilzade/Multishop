namespace MultiShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } 
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal CostPrice { get; set; }
        
        //===Relation Props===//
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
        public List<Image>? Images { get; set; }
    }
}
