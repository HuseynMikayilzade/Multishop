using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MultiShop.Areas.Manage.Utilities.Extentions;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public SlideController(AppDbContext context, IWebHostEnvironment environment, IMapper mapper)
        {
            _context = context;
            _env = environment;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.OrderByDescending(x => x.Id).ToListAsync();
            PaginationVm<Slide> paginationvm = new PaginationVm<Slide>
            {
                Items = slides,

            };
            return View(paginationvm);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (!vm.Photo.CheckSize(3))
            {
                ModelState.AddModelError("Photo", "Size Incorrect");
                return View(vm);
            }
            if (!vm.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "Type Incorrect");
                return View(vm);
            }
            string filename = await vm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
            Slide slide = _mapper.Map<Slide>(vm);
            slide.İmage = filename;
            _context.Slides.Add(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide exist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            UpdateSlideVm vm = _mapper.Map<UpdateSlideVm>(exist);
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVm vm)
        {
            Slide exist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (vm.Photo != null)
            {
                if (!vm.Photo.CheckSize(3))
                {
                    ModelState.AddModelError("Photo", "Size incorrect");
                    return View(vm);
                }
                if (!vm.Photo.CheckType("image/"))
                {
                    ModelState.AddModelError("Photo", "Type incorrect");
                    return View(vm);
                }
                string filename = await vm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
                if (exist.İmage != null) exist.İmage.DeleteFile(_env.WebRootPath, "assets", "img");

                exist.İmage = filename;
            }
            _mapper.Map(vm, exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide exist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            if (exist.İmage != null) exist.İmage.DeleteFile(_env.WebRootPath, "assets", "img");
            _context.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
