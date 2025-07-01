using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CouponController : Controller
    {
        private readonly AppDbContext _context;
        public CouponController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0) return BadRequest();
            double count = await _context.Coupons.CountAsync();
            if (count < 0) return NotFound();
            double totalpage = Math.Ceiling(count / 5);

            List<Coupon> coupons = await _context.Coupons.OrderByDescending(x => x.Id).Skip((page - 1) * 5).Take(5).ToListAsync();
            //List<Size> sizes = await _context.Sizes.ToListAsync();
            if (coupons == null) return NotFound();

            PaginationVm<Coupon> vm = new PaginationVm<Coupon>
            {
                Items = coupons,
                TotalPage = totalpage,
                CurrentPage = page
            };
            return View(vm);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCouponVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _context.Coupons.AnyAsync(s => s.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", " This coupon is aviable");
                return View(vm);
            }
            Coupon coupon = new Coupon
            {
                Name = vm.Name.Trim().ToUpper(),
                Value = vm.Value,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                isActive = vm.isActive,
                CountUsePerUser = vm.CountUsePerUser,
                TotalUsageLimit = vm.TotalUsageLimit,
            };
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Coupon coupon = await _context.Coupons.FirstOrDefaultAsync(s => s.Id == id);
            if (coupon == null) NotFound();

            UpdateCouponVm vm = new UpdateCouponVm
            {
                Name = coupon.Name,
                Value = coupon.Value,
                isActive = coupon.isActive,
                StartDate= coupon.StartDate,
                EndDate = coupon.EndDate,
                CountUsePerUser= coupon.CountUsePerUser,
                CurrentUsageCount = coupon.CurrentUsageCount,
                TotalUsageLimit = coupon.TotalUsageLimit,                    
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateCouponVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Coupon coupon = await _context.Coupons.FirstOrDefaultAsync(s => s.Id == id);
            if (coupon == null) NotFound();

            coupon.Name = vm.Name.Trim().ToUpper();
            coupon.Value = vm.Value;
            coupon.isActive = vm.isActive;
            coupon.StartDate = vm.StartDate;
            coupon.EndDate = vm.EndDate;
            coupon.CountUsePerUser = vm.CountUsePerUser;
            coupon.CurrentUsageCount = vm.CurrentUsageCount;
            coupon.TotalUsageLimit = vm.TotalUsageLimit;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Coupon exist = await _context.Coupons.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            _context.Coupons.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
