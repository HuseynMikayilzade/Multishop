using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.ViewModels;

namespace MultiShop.Services
{
    public class CouponService
    {
        private readonly AppDbContext _context;
        public  CouponService(AppDbContext context)
        {
            _context = context;
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
                .CountAsync(x => x.CouponId == coupon.Id && x.UserId == userId);
            if (userUsageCount >= coupon.CountUsePerUser)
                return new CouponResultVm { IsValid = false, Message = "Bu kupondan artıq istifadə etmisiniz." };

            return new CouponResultVm
            {
                IsValid = true,
                Value =(decimal) coupon.Value,
                CouponId = coupon.Id
            };
        }
    }
}
