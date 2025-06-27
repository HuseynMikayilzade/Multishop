using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;
using System.Diagnostics;

namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.OrderBy(s => s.Order).ToListAsync();
            List<SpecialProduct> specialProducts = await _context.SpecialProducts.OrderBy(s => s.Order).Take(2).ToListAsync();
            List<CustomService> customServices = await _context.CustomServices.Take(4).ToListAsync();
            List<Category> categories = await _context.Categories.Include(c=>c.Product).ToListAsync();
            List<Product> products = await _context.Products
                .Include(p=>p.Images)
                .Include(p=>p.Category)
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes).ThenInclude(pc=>pc.size)
                .ToListAsync();
           
            HomeVm vm = new HomeVm
            {
                Slides = slides,
                SpecialProducts = specialProducts,
                CustomServices = customServices,
                Categories = categories,
                Products = products,
                
            };
            return View(vm);
        }
    }
}