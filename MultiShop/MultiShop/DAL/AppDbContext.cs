using Microsoft.EntityFrameworkCore;
using MultiShop.Models;

namespace MultiShop.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Slide> Slides { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set;}
        public DbSet<Color> Colors { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public DbSet<SpecialProduct> SpecialProducts { get; set; }
        public DbSet<CustomService> CustomServices { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
