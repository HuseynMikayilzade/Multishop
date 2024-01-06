using MultiShop.Models;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateProductVm
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal CostPrice { get; set; }

        //===Relation Props===//
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int> ColorIds { get; set; }
        public List<Color>? Colors { get; set; }
        public List<int> SizeIds { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<int>? ImageIds { get; set; }
        public List<Image>? Images { get; set; }
        public IFormFile? Photo { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
    }
}
