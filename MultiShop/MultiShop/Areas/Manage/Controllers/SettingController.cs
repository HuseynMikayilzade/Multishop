using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page=1)
        {            
            if (page <= 0) return BadRequest();
            double count = await _context.Settings.CountAsync();
            if (count <0 ) return NotFound();

            double totalpage = Math.Ceiling((double)count / 5);
            List<Setting> settings = await _context.Settings.ToListAsync(); //skip , take duzgun islemir tamamlayacam (countu duzgun gelmir deye)//
            PaginationVm<Setting> vm = new PaginationVm<Setting>
            {
                Items=settings,
                CurrentPage=page,
                TotalPage=totalpage,
            };
            return View(vm);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSettingVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if(await _context.Settings.AnyAsync(s=>s.Key == vm.Key))
            {
                ModelState.AddModelError("Key", "This key is aviable");
                return View(vm);
            }
            Setting setting = new Setting
            {
                Key= vm.Key,
                Value= vm.Value,
            };
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Setting exist =await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();

            UpdateSettingVm vm = new UpdateSettingVm
            {
                Key = exist.Key,
                Value = exist.Value,
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSettingVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Setting exist = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
           
            exist.Key = vm.Key;
            exist.Value = vm.Value;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Setting exist = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            _context.Settings.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
