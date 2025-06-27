using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page= 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Sizes.CountAsync();
            if (count < 0) return NotFound();
            double totalpage = Math.Ceiling(count / 5);

            List<Size> sizes =await _context.Sizes.Skip((page-1)*5).Take(5).ToListAsync();
            //List<Size> sizes = await _context.Sizes.ToListAsync();
            if(sizes == null) return NotFound();

            PaginationVm<Size> vm = new PaginationVm<Size>
            {
                Items = sizes,
                TotalPage = totalpage,
                CurrentPage = page
            };
            return View(vm);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _context.Sizes.AnyAsync(s=>s.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", " This size is aviable");
                return View(vm);
            }
            Size size = new Size
            {
                Name = vm.Name.ToUpper(),
            };
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id )
        {
            if (id <= 0) return BadRequest();
            Size exist = _context.Sizes.FirstOrDefault(s=>s.Id == id);
            if (exist == null) return NotFound();

            UpdateSizeVm vm = new UpdateSizeVm
            {
                Name = exist.Name,
            };
           
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateSizeVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Size exist = _context.Sizes.FirstOrDefault(s => s.Id == id);
            if (exist == null) return NotFound();

            exist.Name = vm.Name.ToUpper();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));   
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Size exist = _context.Sizes.FirstOrDefault(s => s.Id == id);
            if (exist == null) return NotFound();
            _context.Sizes.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
