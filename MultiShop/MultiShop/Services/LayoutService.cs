using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
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
    }
}
