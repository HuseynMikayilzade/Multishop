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
    public class WishController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishController(AppDbContext context, UserManager<AppUser> manager)
        {
            _context = context;
            _userManager = manager;
        }

        public async Task<IActionResult> Index()
        {
            List<WishListItemVm> itemvm = new List<WishListItemVm>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.WishListItems.Where(bi => bi.isLiked == true))
                    .ThenInclude(bi => bi.Product).ThenInclude(p => p.Images.Where(pi => pi.isPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) throw new NotFoundException("User Not Found ");

                foreach (var item in appUser.WishListItems)
                {
                    itemvm.Add(new WishListItemVm
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Image = item.Product.Images.FirstOrDefault()?.Url,
                        Description = item.Product.Description,
                        Discount = item.Product.Discount,
                        Price = item.Product.Price,
                    });

                }
            }
            else
            {
                if (Request.Cookies["Wish"] != null)
                {
                    List<WishCookieItemVm> cookies = JsonConvert.DeserializeObject<List<WishCookieItemVm>>(Request.Cookies["Wish"]);
                    if (cookies != null)
                    {
                        foreach (var item in cookies)
                        {
                            Product product = await _context.Products.Include(p => p.Images.Where(i => i.isPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                            if (product == null) return NotFound();
                            if (cookies != null)
                            {

                                itemvm.Add(new WishListItemVm
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Image = product.Images.FirstOrDefault().Url,
                                });
                            }
                        }
                    }
                }
            }
            return View(itemvm);
        }

        public async Task<IActionResult> AddWishList(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found ");

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                AppUser appuser = await _userManager.Users
                    .Include(u => u.WishListItems)
                        .ThenInclude(w => w.Product)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (appuser == null) throw new NotFoundException("User Not Found ");

                WishListItem WishListItem = appuser.WishListItems.FirstOrDefault(p => p.ProductId == product.Id);
                WishListItem = new WishListItem
                {
                    AppUserId = appuser.Id,
                    ProductId = product.Id,
                    Price = product.Price,
                    Description = product.Description,
                    Discount = product.Discount,
                    isLiked = true
                };
                appuser.WishListItems.Add(WishListItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                List<WishCookieItemVm> wish;
                if (Request.Cookies["Wish"] is null)
                {
                    wish = new List<WishCookieItemVm>();
                    WishCookieItemVm WishCookieItemVm = new WishCookieItemVm
                    {
                        Id = id,
                        Count = 1
                    };
                    wish.Add(WishCookieItemVm);
                }
                else
                {
                    wish = JsonConvert.DeserializeObject<List<WishCookieItemVm>>(Request.Cookies["Wish"]);
                    WishCookieItemVm existed = wish.FirstOrDefault(b => b.Id == id);

                    if (existed == null)
                    {
                        WishCookieItemVm WishCookieItemVm = new WishCookieItemVm
                        {
                            Id = id,
                            Count = 1
                        };
                        wish.Add(WishCookieItemVm);
                    }
                    else
                    {
                        existed.Count++;
                    }

                }
                string json = JsonConvert.SerializeObject(wish);
                Response.Cookies.Append("Wish", json);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

    }
}
