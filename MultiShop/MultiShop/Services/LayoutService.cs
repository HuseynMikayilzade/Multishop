using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Dictionary<string,string>> GetSettingAsync()
        {
            Dictionary<string, string> setting = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return setting;
        }
        public async Task<List<Category>> GetCategoryAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }
        public async Task<int> GetBasketCount()
        {
            int count = 0;
            var ContextAccessor = _httpContextAccessor.HttpContext?.User;
            // Authenticated user
            if (ContextAccessor?.Identity?.IsAuthenticated == true)
            {
                string userId = ContextAccessor.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    count = await _context.BasketItems
                        .Where(b => b.AppUserId == userId && b.OrderId == null)
                        .SumAsync(b => b.Count);
                }
            }
            else // Guest user (cookie-based basket)
            {
                var context = new HttpContextAccessor().HttpContext;
                string cartCookie = context.Request.Cookies["Cart"];

                if (!string.IsNullOrEmpty(cartCookie))
                {
                    var basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(cartCookie);
                    count = basket?.Sum(b => b.Count) ?? 0;
                }
            }

            return count;
        }
        public async Task<int> GetWishCount()
        {
            int count = 0;
            var ContextAccessor = _httpContextAccessor.HttpContext?.User;
            // Authenticated user
            if (ContextAccessor?.Identity?.IsAuthenticated == true)
            {
                string userId = ContextAccessor.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    count = await _context.WishListItems
                        .Where(b => b.AppUserId == userId && b.isLiked == true).CountAsync();
                }
            }
            else // Guest user (cookie-based basket)
            {
                var context = new HttpContextAccessor().HttpContext;
                string wishCookie = context.Request.Cookies["Wish"];

                if (!string.IsNullOrEmpty(wishCookie))
                {
                    var wishlist = JsonConvert.DeserializeObject<List<WishCookieItemVm>>(wishCookie);
                    count = wishlist?.Sum(b => b.Count) ?? 0;
                }
            }

            return count;
        }

    }
}
