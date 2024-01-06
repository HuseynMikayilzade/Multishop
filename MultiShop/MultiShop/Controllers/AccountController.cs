using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extentions;
using MultiShop.ViewModels;

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
            

            return RedirectToAction(nameof(Index), "Home");
        }

    }
}
