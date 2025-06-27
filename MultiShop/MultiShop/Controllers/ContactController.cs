using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Controllers
{
    public class ContactController : Controller
    {
        public ContactController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
