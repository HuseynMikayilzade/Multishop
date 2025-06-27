using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) throw new Exception("bad request");
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.size)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new Exception("Not Found");

            List<Category> categories = await _context.Categories.ToListAsync();
            if (categories == null) throw new Exception("Not Found");

            List<Product> relatedproducts = await _context.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToListAsync();
            if (relatedproducts == null) throw new Exception("Not Found");
            ProductVm vm = new ProductVm
            {
                Product = product,
                Categories = categories,
                RelatedProducts = relatedproducts
            };

            return View(vm);
        }

        public async Task<IActionResult> AddBasket(int id, int count, string? returnUrl, int sizeid, int colorid)
        {
            if (count <= 0)
                throw new BadRequestException("Count must be greater than zero.");

            if (sizeid <= 0)
                throw new BadRequestException("Size ID is invalid.");

            if (colorid <= 0)
                throw new BadRequestException("Color ID is invalid.");

            if (id <= 0)
                throw new BadRequestException("Product ID is invalid.");

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found ");

            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appuser == null) throw new NotFoundException("User Not Found ");
                BasketItem basketItem = appuser.BasketItems.FirstOrDefault(p => p.ProductId == product.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = appuser.Id,
                        ProductId = product.Id,
                        Count = count,
                        Price = product.Price,
                        ColorId = colorid,
                        SizeId = sizeid,
                    };
                    appuser.BasketItems.Add(basketItem);
                }
                else
                {
                    // Eyni məhsul, eyni rəng və ölçü yoxdursa, yeni əlavə etməlisən
                    if (basketItem.ColorId == colorid && basketItem.SizeId == sizeid)
                    {
                        basketItem.Count += count;
                    }
                    else
                    {
                        // Əgər fərqli ölçü və rənglə yenidən eyni məhsul əlavə olunursa, ayrı bir basketItem yaradılmalıdır:
                        BasketItem newItem = new BasketItem
                        {
                            AppUserId = appuser.Id,
                            ProductId = product.Id,
                            Count = count,
                            Price = product.Price,
                            ColorId = colorid,
                            SizeId = sizeid
                        };
                        appuser.BasketItems.Add(newItem);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVm> cart;

                if (Request.Cookies["Cart"] is null)
                {
                    cart = new List<BasketCookieItemVm>();
                    BasketCookieItemVm basketCookieItem = new BasketCookieItemVm
                    {
                        Id = id,
                        Count = count,
                        ColorId = colorid,
                        SizeId = sizeid
                    };
                    cart.Add(basketCookieItem);
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Cart"]);

                    // Eyni məhsul, eyni rəng və ölçü yoxdursa yeni əlavə et, varsa sayını artır
                    BasketCookieItemVm existed = cart.FirstOrDefault(b => b.Id == id && b.ColorId == colorid && b.SizeId == sizeid);

                    if (existed == null)
                    {
                        BasketCookieItemVm basketCookieItemVm = new BasketCookieItemVm
                        {
                            Id = id,
                            Count = count,
                            ColorId = colorid,
                            SizeId = sizeid
                        };
                        cart.Add(basketCookieItemVm);
                    }
                    else
                    {
                        existed.Count += count;
                    }
                }

                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("Cart", json);
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
