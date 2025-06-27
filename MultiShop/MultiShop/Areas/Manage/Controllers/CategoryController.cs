using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.Utilities.Extentions;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _env = environment;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Categories.CountAsync();
            if(count<0) return NotFound();
            double totalpage = Math.Ceiling(count / 5);


            List<Category> categories = await _context.Categories.Skip((page-1)*5).Take(5).ToListAsync();
            //List<Category> categories = await _context.Categories.ToListAsync();
            if(categories==null) return NotFound();

            PaginationVm<Category> vm = new PaginationVm<Category>
            {
                Items = categories,
                TotalPage =totalpage,
                CurrentPage = page,
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (await _context.Categories.AnyAsync(c => c.Name.Trim() == vm.Name.Trim()))
            {
                ModelState.AddModelError("Name", "This Category is aviable");
                return View(vm);
            }

            if (vm.Photo == null) throw new Exception("Photo is required");

            if (!vm.Photo.CheckSize(3))
            {
                ModelState.AddModelError("Photo", "Photo size in correct");
                return View(vm);
            }
            if (!vm.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "Photo type in correct");
                return View(vm);
            }

            string filename = await vm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
            Category category = _mapper.Map<Category>(vm);
            category.Image = filename;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category exist = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (exist == null) throw new Exception(" Category Not Found");

            UpdateCategoryVm vm = _mapper.Map<UpdateCategoryVm>(exist);

            return View(vm);

        }
        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateCategoryVm vm)
        {
            Category exist = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (exist == null) throw new Exception("Category Not Found");

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.Photo != null)
            {
                if (!vm.Photo.CheckType("image/"))
                {
                    ModelState.AddModelError("Photo", "Photo type in correct");
                    return View(vm);
                }
                if (!vm.Photo.CheckSize(3))
                {
                    ModelState.AddModelError("Photo", "Photo size in correct");
                    return View(vm);
                }
                string filename = await vm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
                if (exist.Image != null) exist.Image.DeleteFile(_env.WebRootPath, "assets", "img");
                exist.Image = filename;
            }
            _mapper.Map(vm, exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return NotFound();
            Category exist = await _context.Categories.FirstOrDefaultAsync(slide => slide.Id == id);
            if (exist == null) return NotFound();
            if (exist.Image != null) exist.Image.DeleteFile(_env.WebRootPath, "assets", "img");

            _context.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
