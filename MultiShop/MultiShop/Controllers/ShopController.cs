using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? categoryid, int page = 1, int number = 12, int order = 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Products.CountAsync();
            if (count <= 0) return NotFound();
            double totalpage = Math.Ceiling((double)(count) / number);
            if (page > totalpage) return BadRequest();

            IQueryable<Product> query;
            if (categoryid == null)
            {
                query = _context.Products
                   .Include(p => p.Images)
                   .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                   .Include(p => p.ProductSizes).ThenInclude(ps => ps.size);
            }
            else
            {
                query = _context.Products.Where(p=>p.CategoryId==categoryid)
                   .Include(p => p.Images)
                   .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                   .Include(p => p.ProductSizes).ThenInclude(ps => ps.size);
            }
            if (query == null) return NotFound();

            switch (order)
            {
                case 1:
                    query = query;
                    break;
                case 2:
                    query = query.OrderBy(q => q.Price);
                    break;
                case 3:
                    query = query.OrderByDescending(q => q.Price);
                    break;
            }
            List<Product> products = await query.Skip((page - 1) * number).Take(number).ToListAsync();
            var result = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryid);
            ShopVm vm = new ShopVm
            {
                Products = products,
                TotalPage = totalpage,
                CurrentPage = page,
                CategoryName = result?.Name
            };
            return View(vm);
        }
    }
}
