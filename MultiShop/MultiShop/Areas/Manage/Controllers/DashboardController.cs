using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;
using System.Threading.Tasks;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public DashboardController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _signInManager.UserManager.GetUserAsync(User);

            return View(user);
        }
    }
}
