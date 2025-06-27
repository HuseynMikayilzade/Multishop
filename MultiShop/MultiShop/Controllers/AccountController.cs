using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extentions;
using MultiShop.ViewModels;
using Newtonsoft.Json;

namespace MultiShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(AppDbContext context,UserManager<AppUser> manager,SignInManager<AppUser> signInManager )
        {
            _context = context;
            _manager = manager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult IsAuthenticated()
        {
            return Json(new { isAuthenticated = User.Identity.IsAuthenticated });
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (CheckRegister.isDigit(vm.Name))
            {
                ModelState.AddModelError("Name", "Number cannot be entered");
                return View(vm);
            }
            if (CheckRegister.isDigit(vm.Surname))
            {
                ModelState.AddModelError("Surname", "Number cannot be entered");
                return View(vm);
            }
            AppUser user = new AppUser
            {
                Name = CheckRegister.Capitalize(vm.Name),
                Surname = CheckRegister.Capitalize(vm.Surname),
                UserName = vm.Username,
                Gender = vm.Gender,
                Email = vm.Email
            };

            var result = await _manager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }
                return View(vm); 
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            AppUser user =await _manager.FindByNameAsync(vm.UserNameOrEmail);
            if (user == null)
            {
                user = await _manager.FindByEmailAsync(vm.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("UserNameOrEmail", "Email,Username or Password incorrect");
                    return View(vm);
                }
            }
            var result = _signInManager.PasswordSignInAsync(user, vm.Password, vm.Isremembered, true);
            if (result.Result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Account is locked. Please try again after a few minutes.");
                return View(vm);
            }
            if (!result.Result.Succeeded)
            {
                ModelState.AddModelError("UserNameOrEmail", "Email,Username or Password incorrect");
                return View(vm);
            }
            // Cookie-dəki məhsulları istifadəçinin basketine əlavə et
            if (Request.Cookies["Cart"] != null)
            {
                List<BasketCookieItemVm> cartItems = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Cart"]);

                if (cartItems != null && cartItems.Count > 0)
                {
                    AppUser User = await _manager.Users
                        .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                        .FirstOrDefaultAsync(u => u.Id == user.Id);

                    foreach (var cookieItem in cartItems)
                    {
                        // Eyni məhsul, rəng və ölçü varsa, sayını artır
                        var existingItem = User.BasketItems.FirstOrDefault(b =>
                            b.ProductId == cookieItem.Id &&
                            b.ColorId == cookieItem.ColorId &&
                            b.SizeId == cookieItem.SizeId);

                        if (existingItem != null)
                        {
                            existingItem.Count += cookieItem.Count;
                        }
                        else
                        {
                            BasketItem newItem = new BasketItem
                            {
                                ProductId = cookieItem.Id,
                                Count = cookieItem.Count,
                                AppUserId = User.Id,
                                Price = (await _context.Products.FirstOrDefaultAsync(p => p.Id == cookieItem.Id))?.Price ?? 0,
                                ColorId = cookieItem.ColorId,
                                SizeId = cookieItem.SizeId
                            };
                            User.BasketItems.Add(newItem);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Cookie təmizlənir çünki artıq DB-yə köçürüldü
                    Response.Cookies.Delete("Cart");
                }
            }
            // Cookie-dəki məhsulları istifadəçinin wishlistinə əlavə et
            if (Request.Cookies["Wish"] != null)
            {
                List<WishListItemVm> wishItems = JsonConvert.DeserializeObject<List<WishListItemVm>>(Request.Cookies["Wish"]);

                if (wishItems != null && wishItems.Count > 0)
                {
                    AppUser dbUser = await _manager.Users
                        .Include(u => u.WishListItems.Where(w => w.isLiked == true))
                        .FirstOrDefaultAsync(u => u.Id == user.Id);

                    foreach (var item in wishItems)
                    {
                        // Əgər artıq wishlist-də varsa, əlavə etmə
                        var existingItem = dbUser.WishListItems.FirstOrDefault(w => w.ProductId == item.Id);

                        if (existingItem == null)
                        {
                            WishListItem newItem = new WishListItem
                            {
                                ProductId = item.Id,
                                AppUserId = dbUser.Id,
                                Price = (await _context.Products.FirstOrDefaultAsync(p => p.Id == item.Id))?.Price ?? 0,
                                isLiked = true,
                                Description = item.Description
                            };
                            dbUser.WishListItems.Add(newItem);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Cookie təmizlənir çünki artıq DB-yə köçürüldü
                    Response.Cookies.Delete("Wish");
                }
            }


            return RedirectToAction(nameof(Index), "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
