using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) throw new Exception("bad request");
            Product product = await _context.Products
                .Include(p=>p.Images)
                .Include(p=>p.ProductSizes).ThenInclude(ps=>ps.size)
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null) throw new Exception("Not Found");

            List<Category> categories = await _context.Categories.ToListAsync();
            if (categories == null) throw new Exception("Not Found");

            List<Product> relatedproducts = await _context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Include(p=>p.Category)
                .Include(p => p.Images)
                .ToListAsync();
            if(relatedproducts==null) throw new Exception("Not Found");
            ProductVm vm = new ProductVm
            {
                Product = product,
                Categories= categories,
                RelatedProducts= relatedproducts
            };

            return View(vm);
        }
    }
}
