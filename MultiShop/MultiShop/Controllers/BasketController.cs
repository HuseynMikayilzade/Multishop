using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Interfaces;
using MultiShop.Models;
using MultiShop.Services;
using MultiShop.Utilities.Exceptions;
using MultiShop.Utilities.Extentions;
using MultiShop.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly CouponService _couponService;
        private readonly IEmailService _emailService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager, CouponService couponService, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _couponService = couponService;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index(string? coupon)
        {
            List<BasketItemVm> itemvm = new List<BasketItemVm>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .ThenInclude(bi => bi.Product).ThenInclude(p => p.Images.Where(pi => pi.isPrimary == true))
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).ThenInclude(x => x.Color)
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).ThenInclude(x => x.Size)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) throw new NotFoundException("User Not Found ");


                foreach (var item in appUser.BasketItems)
                {
                    itemvm.Add(new BasketItemVm
                    {
                        Id = item.ProductId,
                        SalePrice = item.Product.Price - item.Product.Discount,
                        Count = item.Count,
                        Name = item.Product.Name,
                        SubTotal = item.Count * (item.Product.Price - item.Product.Discount),
                        Price = item.Product.Price,
                        Image = item.Product.Images.FirstOrDefault()?.Url,
                        Color = item.Color?.Name ?? "N/A",
                        Size = item.Size?.Name ?? "N/A"
                    });

                }
            }
            else
            {
                if (Request.Cookies["Cart"] != null)
                {
                    List<BasketCookieItemVm> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Cart"]);
                    if (cookies != null)
                    {
                        foreach (var item in cookies)
                        {
                            Product product = await _context.Products.Include(p => p.Images.Where(i => i.isPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                            if (product == null) return NotFound();
                            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == item.SizeId);
                            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == item.ColorId);

                            if (cookies != null)
                            {
                                var saleprice = product.Price - product.Discount;
                                itemvm.Add(new BasketItemVm
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    SalePrice = saleprice,
                                    Count = item.Count,
                                    SubTotal = item.Count * saleprice,
                                    Price = product.Price,
                                    Image = product.Images.FirstOrDefault().Url,
                                    Color = color?.Name ?? "N/A",
                                    Size = size?.Name ?? "N/A"
                                });
                            }
                        }
                    }
                }
            }
            return View(itemvm);
        }


        public async Task<IActionResult> AddBasket(int id, string? returnUrl)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
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
                        Count = 1,
                        Price = product.Price - product.Discount,
                    };
                    appuser.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
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
                        Count = 1
                    };
                    cart.Add(basketCookieItem);
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Cart"]);
                    BasketCookieItemVm existed = cart.FirstOrDefault(b => b.Id == id);

                    if (existed == null)
                    {
                        BasketCookieItemVm basketCookieItemVm = new BasketCookieItemVm
                        {
                            Id = id,
                            Count = 1,
                        };
                        cart.Add(basketCookieItemVm);
                    }
                    else
                    {
                        existed.Count++;
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

        [HttpPost]
        public async Task<IActionResult> UpdateCookie(int id, int count)
        {
            if (id <= 0 || count <= 0) return BadRequest();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appUser == null) return NotFound("User not found");

                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item != null)
                {
                    item.Count = count;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound("Basket item not found");
                }
            }
            else
            {
                var cookie = Request.Cookies["Cart"];
                if (cookie != null)
                {
                    List<BasketCookieItemVm> items = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(cookie);
                    BasketCookieItemVm item = items.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        item.Count = count;
                        string updatedCart = JsonConvert.SerializeObject(items);
                        Response.Cookies.Append("Cart", updatedCart);
                    }
                    else
                    {
                        return NotFound("Item not found in cookie");
                    }
                }
                else
                {
                    return NotFound("Cart not found");
                }
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found :(");

            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appuser == null) throw new NotFoundException("User Not Found :(");
                BasketItem basketItem = appuser.BasketItems.FirstOrDefault(b => b.ProductId == product.Id);
                if (basketItem == null) throw new NotFoundException("An Unexpected Error Occurred :(");
                appuser.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Cart"]);
                if (basket == null) throw new NotFoundException("An Unexpected Error Occurred :(");

                BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);
                if (existed == null) throw new NotFoundException("An Unexpected Error Occurred :(");
                basket.Remove(existed);
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Cart", json);

            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> CheckOut(string? coupon)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(bi => bi.Product).ThenInclude(p => p.Images.Where(i => i.isPrimary == true))
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(bi => bi.Color)
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(bi => bi.Size).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (appUser == null) throw new NotFoundException("User Not Found :(");
            if (!appUser.BasketItems.Any())
                return RedirectToAction("Error", "Basket", new { errormessage = "Basket is empty", returnUrl = Url.Action("Index", "Basket") });

            decimal discount = 0;
            if (!string.IsNullOrEmpty(coupon))
            {
                var result = await _couponService.ApplyCouponAsync(coupon, appUser.Id);
                if (result.IsValid) discount = result.Value;

            }

            var SubTotal = appUser.BasketItems.Sum(x => (x.Product.Price - x.Product.Discount) * x.Count);
            OrderVm ordervm = new OrderVm
            {
                BasketItems = appUser.BasketItems,
                Name = appUser.Name,
                Surname = appUser.Surname,
                Email = appUser.Email,
                SubTotal = SubTotal,
                CouponDiscount = discount,
                Total = SubTotal - discount,
                Coupon = coupon,
            };
            return View(ordervm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CheckOut(OrderVm orderVm)
        {
            AppUser appUser = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).ThenInclude(bi => bi.Product).ThenInclude(p => p.Images.Where(i => i.isPrimary == true))
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(bi => bi.Color)
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(bi => bi.Size).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (appUser == null) return NotFound("İstifadəçi tapılmadı");

            if (!ModelState.IsValid)
            {
                orderVm.BasketItems = appUser.BasketItems;
                return View(orderVm);
            }

            orderVm.Total = orderVm.SubTotal - orderVm.CouponDiscount;
            var order = new Order
            {
                Address = orderVm.Adress,
                AppUserId = appUser.Id,
                Status = null,
                Received = DateTime.Now,
                BasketItems = appUser.BasketItems,
                TotalPrice = orderVm.Total,
                CouponDiscount = orderVm.CouponDiscount,
            };
            if (!string.IsNullOrEmpty(orderVm.Coupon))
            {
                await _couponService.AppliedCouponUsageAsync(orderVm.Coupon, appUser.Id);
            }
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            string body = EmailCreator.EmailBody(order);
            await _emailService.SendEmailAsync(appUser.Email, "Order confirmation", body, true);
            return RedirectToAction(nameof(Success), new { returnUrl = Url.Action("Index", "Basket") });

        }
        public IActionResult Success(string? returnUrl)
        {
            SuccesVm succesVm = new SuccesVm
            {
                Message = "Your order has been completed successfully. Thanks!",
                ReturnUrl = returnUrl ?? Request.Headers["Referer"].ToString(),
            };
            return View(succesVm);
        }
        public IActionResult Error(string errormessage, string returnUrl)
        {
            ErrorVm vm = new ErrorVm();
            vm.Message = string.IsNullOrEmpty(errormessage)
                       ? "An error occurred while placing your order. Please try again later."
                       : errormessage;
            vm.ReturnUrl = returnUrl;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            var result = await _couponService.ApplyCouponAsync(code, userId);

            if (!result.IsValid)
                return Json(new { success = false, message = result.Message });

            var items = await GetBasketItems(userId);
            decimal subtotal = items.Sum(x => x.SubTotal);
            decimal discount = result.Value;
            decimal total = subtotal - discount;

            return Json(new
            {
                success = true,
                discount,
                message = "Kupon tətbiq olundu",
                subtotal,
                total
            });
        }
        private async Task<List<BasketItemVm>> GetBasketItems(string userId)
        {
            List<BasketItemVm> itemvm = new();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems.Where(b => b.OrderId == null))
                    .ThenInclude(b => b.Product)
                    .ThenInclude(p => p.Images.Where(i => i.isPrimary == true))
                    .FirstOrDefaultAsync(x => x.Id == userId);

                foreach (var item in appUser.BasketItems)
                {
                    itemvm.Add(new BasketItemVm
                    {
                        Id = item.ProductId,
                        Price = item.Product.Price - item.Product.Discount,
                        Count = item.Count,
                        Name = item.Product.Name,
                        SubTotal = item.Count * (item.Product.Price - item.Product.Discount),
                        Image = item.Product.Images.FirstOrDefault()?.Url
                    });
                }
            }
            else
            {
                var cookie = Request.Cookies["Cart"];
                if (cookie != null)
                {
                    var cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(cookie);
                    foreach (var item in cookies)
                    {
                        var product = await _context.Products.Include(p => p.Images.Where(i => i.isPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (product == null) continue;

                        itemvm.Add(new BasketItemVm
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price - product.Discount,
                            Count = item.Count,
                            SubTotal = item.Count * (product.Price - product.Discount),
                            Image = product.Images.FirstOrDefault()?.Url
                        });
                    }
                }
            }

            return itemvm;
        }

    }
}
