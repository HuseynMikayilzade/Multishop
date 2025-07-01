using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Services
{
    public class CouponService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CouponService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CouponResultVm> ApplyCouponAsync(string couponCode, string userId)
        {

            var coupon = await _context.Coupons
                .Include(c => c.CouponUsages)
                .FirstOrDefaultAsync(c => c.Name.Trim().ToUpper() == couponCode.Trim().ToUpper() && c.isActive);

            if (coupon == null)
                return new CouponResultVm { IsValid = false, Message = "Belə bir kupon mövcud deyil." };

            if (DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate)
                return new CouponResultVm { IsValid = false, Message = "Kuponun müddəti bitmişdir." };

            if (coupon.CurrentUsageCount >= coupon.TotalUsageLimit)
                return new CouponResultVm { IsValid = false, Message = "Kupon istifadəsi limitə çatıb." };

            int userUsageCount = await _context.CouponsUsages
                .CountAsync(x => x.CouponId == coupon.Id && x.AppUserId == userId);
            if (userUsageCount >= coupon.CountUsePerUser)
                return new CouponResultVm { IsValid = false, Message = "Bu kupondan artıq istifadə etmisiniz." };

            return new CouponResultVm
            {
                IsValid = true,
                Value = (decimal)coupon.Value,
                CouponId = coupon.Id
            };
        }

        public async Task AppliedCouponUsageAsync(string couponCode, string userId)
        {
            var coupon = await _context.Coupons
              .Include(c => c.CouponUsages)
              .FirstOrDefaultAsync(c => c.Name.Trim().ToUpper() == couponCode.Trim().ToUpper() && c.isActive);
            if (coupon == null) throw new Exception("Failed");
           
            coupon.CurrentUsageCount++;

            CouponUsage couponUsage = new CouponUsage
            {
                AppUserId= userId,
                CouponId = coupon.Id,
                UsedAt = DateTime.Now,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            };
            await _context.CouponsUsages.AddAsync(couponUsage);
            await _context.SaveChangesAsync();
        }
    }
}
