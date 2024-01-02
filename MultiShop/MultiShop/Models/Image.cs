namespace MultiShop.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool? isPrimary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
