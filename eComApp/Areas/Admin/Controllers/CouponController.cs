using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class CouponController : Controller
    {
        private readonly IRepo<Coupon> _couponRepo;
        private readonly IRepo<Category> _categeoryRepo;
        CouponController(IRepo<Coupon> couponRepo, IRepo<Category> categoryRepo)
        {
            _couponRepo = couponRepo;
            _categeoryRepo = categoryRepo;
        }
        [Authorize]
        public IActionResult Index()
        {
            List<Coupon> Coupons = _couponRepo.GetAll().ToList();
            return View(Coupons);
        }

        public IActionResult Upsert(int Id)
        {
            CouponVM couponVM = new CouponVM();
            couponVM.Categories = _categeoryRepo.GetAll().ToList();
            if (Id!=0) //Edit
            {
                Coupon coupon = _couponRepo.Get(c=>c.Id==Id);
                if (coupon != null)
                {
                    couponVM.Id = coupon.Id;
                    couponVM.ExpirationDate = coupon.ExpirationDate;
                    couponVM.UsageLimit = coupon.UsageLimit;
                    couponVM.Code = coupon.Code;
                    couponVM.Description = coupon.Description;
                    couponVM.IsActive = coupon.IsActive;
                    couponVM.IsSingleUse = coupon.IsSingleUse;
                    couponVM.DiscountType = coupon.DiscountType;
                    couponVM.MaxDiscountAmount = coupon.MaxDiscountAmount;
                    couponVM.MinPurchaseAmount = coupon.MinPurchaseAmount;
                    couponVM.ApplicableCategories = coupon.ApplicableCategories;
                }
            }

            return View(couponVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Upsert(CouponVM couponVM)
        {
            if(ModelState.IsValid)
            {
                if (couponVM.Id == 0) //New Coupon
                {
                    Coupon coupon = new Coupon
                    {
                        ExpirationDate = couponVM.ExpirationDate,
                        UsageLimit = couponVM.UsageLimit,
                        Code = couponVM.Code,
                        Description = couponVM.Description,
                        IsActive = couponVM.IsActive,
                        IsSingleUse = couponVM.IsSingleUse,
                        DiscountType = couponVM.DiscountType,
                        MaxDiscountAmount = couponVM.MaxDiscountAmount,
                        MinPurchaseAmount = couponVM.MinPurchaseAmount,
                        ApplicableCategories = couponVM.ApplicableCategories
                    };
                    _couponRepo.add(coupon);
                }
                else //Edit
                {
                    if(_couponRepo.HasChanges())
                    {
                        Coupon coupon = _couponRepo.Get(c=>c.Id==couponVM.Id);
                        if(coupon != null)
                        {
                            coupon.ExpirationDate = couponVM.ExpirationDate;
                            coupon.UsageLimit = couponVM.UsageLimit;
                            coupon.Code = couponVM.Code;
                            coupon.Description = couponVM.Description;
                            coupon.IsActive = couponVM.IsActive;
                            coupon.IsSingleUse = couponVM.IsSingleUse;
                            coupon.DiscountType = couponVM.DiscountType;
                            coupon.MaxDiscountAmount = couponVM.MaxDiscountAmount;
                            coupon.MinPurchaseAmount = couponVM.MinPurchaseAmount;
                            coupon.ApplicableCategories = couponVM.ApplicableCategories;
                        }
                    }
                }
                _couponRepo.SaveChanges();
            }
            return View(couponVM);
        }
        [Authorize]
        public IActionResult DeleteCoupon(int Id)
        {
            Coupon coupon = _couponRepo.Get(c=>c.Id==Id);
            if (coupon == null)
            {
                _couponRepo.remove(coupon);
                _couponRepo.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}

