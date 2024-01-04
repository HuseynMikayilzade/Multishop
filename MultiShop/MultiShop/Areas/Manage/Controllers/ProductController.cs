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
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Products.CountAsync();
            if (count <= 0) return NotFound();
            double totalpage = Math.Ceiling((double)count / 5);
            if (page > totalpage) return BadRequest();

            List<Product> products = await _context.Products.Include(p => p.Images).ToListAsync();
            if (products == null) return NotFound();
            PaginationVm<Product> vm = new PaginationVm<Product>
            {
                Items = products,
                TotalPage = totalpage,
                CurrentPage = page
            };
            return View(vm);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVm vm = new CreateProductVm();
            vm.Categories = await _context.Categories.ToListAsync();
            vm.Colors = await _context.Colors.ToListAsync();
            vm.Sizes = await _context.Sizes.ToListAsync();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm vm)
        {
            vm.Categories = await _context.Categories.ToListAsync();
            vm.Colors = await _context.Colors.ToListAsync();
            vm.Sizes = await _context.Sizes.ToListAsync();
            if (!ModelState.IsValid) return View(vm);

            if (vm.Price < 0)
            {
                ModelState.AddModelError("Price", "Price incorrect");
                return View(vm);
            }
            if (!await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
            {
                ModelState.AddModelError("Category", "The category isn't aviable");
                return View(vm);
            }
            foreach (var item in vm.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(c => c.Id == item))
                {
                    ModelState.AddModelError("Color", "The color isn't aviable");
                    return View(vm);
                }
            }
            foreach (var item in vm.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("Size", "The size isn't aviable");
                    return View(vm);
                }
            }


            Product product = new Product
            {
                Name = vm.Name,
                Price = vm.Price,
                Description = vm.Description,
                CostPrice = vm.CostPrice,
                Discount = vm.Discount,
                CategoryId = vm.CategoryId,
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>(),
                Images = new List<Image>(),
            };

            if (vm.Photo != null)
            {
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
                product.Images.Add(new Image
                {
                    isPrimary = true,
                    Url = await vm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img")
                });


            }

            if (vm.AdditionalPhotos != null)
            {
                foreach (var item in vm.AdditionalPhotos)
                {
                    if (!item.CheckType("image/"))
                    {
                        ModelState.AddModelError("Additional", "Photo type in correct");
                        return View(vm);
                    }
                    if (!item.CheckSize(3))
                    {
                        ModelState.AddModelError("Additional", "Photo size in correct");
                        return View(vm);
                    }
                    product.Images.Add(new Image
                    {
                        isPrimary = null,
                        Url = await item.CreateFileAsync(_env.WebRootPath, "assets", "img")
                    });
                }
            }

            foreach (var item in vm.ColorIds)
            {
                ProductColor color = new ProductColor
                {
                    ColorId = item,
                };
                product.ProductColors.Add(color);
            }
            foreach (var item in vm.SizeIds)
            {
                ProductSize size = new ProductSize
                {
                    SizeId = item,
                };
                product.ProductSizes.Add(size);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
