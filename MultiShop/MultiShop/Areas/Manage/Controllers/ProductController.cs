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
            if (count < 0) return NotFound();
            double totalpage = Math.Ceiling((double)count / 5);

            List<Product> products = await _context.Products.OrderByDescending(x=>x.Id).Skip((page-1)*5).Take(5).Include(p => p.Images).ToListAsync();
            // List<Product> products = await _context.Products.ToListAsync();
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
                        isPrimary = false,
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




        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return BadRequest();

            UpdateProductVm vm = new UpdateProductVm
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CostPrice = product.CostPrice,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync(),

                ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList(),
                Colors = await _context.Colors.ToListAsync(),

                SizeIds = product.ProductSizes.Select(p => p.SizeId).ToList(),
                Sizes = await _context.Sizes.ToListAsync(),
                Images = product.Images
            };
            return View(vm);
        }
        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateProductVm vm)
        {
            Product exist = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (exist == null) return NotFound();
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
                    ModelState.AddModelError("Size", "The Size isn't aviable");
                    return View(vm);
                }
            }


            if (vm.ColorIds!=null)
            {
                exist.ProductColors.RemoveAll(pc => !vm.ColorIds.Exists(ci => ci == pc.ColorId));
                foreach (var item in vm.ColorIds)
                {
                    if (!exist.ProductColors.Any(pc => pc.ColorId == item))
                    {
                        exist.ProductColors.Add(new ProductColor
                        {
                                ColorId= item,
                        });
                    }
                }
            }

            if (vm.SizeIds!=null)
            {
                exist.ProductSizes.RemoveAll(ps => !vm.SizeIds.Exists(si => si == ps.SizeId));
                foreach (var item in vm.SizeIds)
                {
                    if (!exist.ProductSizes.Any(ps=>ps.SizeId==item))
                    {
                        exist.ProductSizes.Add(new ProductSize
                        {
                            SizeId = item,
                        });
                    }
                }
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
                Image photo = exist.Images.FirstOrDefault(i => i.isPrimary == true);
                if (photo != null) photo.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                exist.Images.Remove(photo);
                exist.Images.Add(new Image { isPrimary = true, Url = filename });
            }


            if (vm.ImageIds==null)
            {
                vm.ImageIds = new List<int>();
            }
            List<Image> removeimages = exist.Images.Where(i=>!vm.ImageIds.Exists(ii=>ii==i.Id)&&i.isPrimary==false).ToList();
            foreach (var item in removeimages)
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                exist.Images.Remove(item);
            }
            if (vm.AdditionalPhotos != null)
            {
                foreach (var item in vm.AdditionalPhotos)
                {
                    if (!item.CheckSize(3))
                    {
                        ModelState.AddModelError("AdditionalPhoto", "Photo size in correct");
                        return View(vm);
                    }
                    if (!item.CheckType("image/"))
                    {
                        ModelState.AddModelError("AdditionalPhoto", "Photo type in correct");
                        return View(vm);
                    }

                    exist.Images.Add(new Image
                    {
                        isPrimary = false,
                        Url = await item.CreateFileAsync(_env.WebRootPath, "assets", "img")
                    });
                    
                }
            }
            exist.Name = vm.Name;
            exist.Description = vm.Description;
            exist.CostPrice= vm.CostPrice;
            exist.Price= vm.Price;
            exist.Discount = vm.Discount;
            exist.CategoryId = vm.CategoryId;
         
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
               .Include(p => p.Images)
               .Include(p => p.ProductColors).ThenInclude(pc=>pc.Color)
               .Include(p => p.ProductSizes).ThenInclude(ps=>ps.size)
               .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            DetailVm<Product> vm = new DetailVm<Product>
            {
                Item = product
            };
            return View(vm);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product exist = _context.Products.FirstOrDefault(s => s.Id == id);
            if (exist == null) return NotFound();
            if (exist.Images!=null)
            {
                foreach (var item in exist.Images)
                {
                    item.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                    exist.Images.Remove(item);
                }
            }
            _context.Products.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
