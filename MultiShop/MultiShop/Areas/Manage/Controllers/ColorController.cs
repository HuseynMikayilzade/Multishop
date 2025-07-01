using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Colors.CountAsync();
            if (count < 0) return NotFound();
            double totalpage = Math.Ceiling(count / 5);
            

            List<Color> colors=await _context.Colors.OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();
           // List<Color> colors = await _context.Colors.ToListAsync();
            PaginationVm<Color> vm = new PaginationVm<Color>
            {
                Items=colors,
                TotalPage=totalpage,
                CurrentPage =page, 
            };
            return View(vm);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _context.Colors.AnyAsync(s => s.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "This color is aviable");
                return View(vm);
            }
            Color color = new Color
            {
                Name = vm.Name,
            };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color exist =await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();

           UpdateColorVm vm =new UpdateColorVm
            {
                Name = exist.Name,
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Color exist =await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();

            exist.Name = vm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Color exist = await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            _context.Colors.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      
    }
}
