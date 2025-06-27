using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiShop.Models;

namespace MultiShop.DAL
{
    public class AppDbContext :IdentityDbContext<AppUser>
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
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponsUsages { get; set; } 
        public DbSet<WishListItem> WishListItems { get; set; }
    }
}
