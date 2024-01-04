using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public List<Product>? Product { get; set; }
       

    }
}
