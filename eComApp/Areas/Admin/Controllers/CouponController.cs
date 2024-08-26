using eCom.DataAccess.Data;
using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.EntityTypeConfiguration;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace eComApp.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class CouponController : Controller
    {
        private readonly IRepo<Coupon> _couponRepo;
        private readonly IRepo<Category> _categeoryRepo;
        private readonly AppDbContext _appDbContext;
        public CouponController(IRepo<Coupon> couponRepo, IRepo<Category> categoryRepo, AppDbContext _appdb)
        {
            _couponRepo = couponRepo;
            _categeoryRepo = categoryRepo;
            _appDbContext = _appdb;
        }
        [Authorize]
        public IActionResult Index()
        {
            List<Coupon> Coupons = _couponRepo.GetAll().ToList();
            return View(Coupons);
        }
        public IActionResult CouponDetailsPV(int Id)
        {
            Coupon Coupon = _couponRepo.Get(c => c.Id == Id);
            if(Coupon!=null)
            {
                return PartialView("~/Areas/Admin/Views/Coupon/PartialViews/_CouponDetailsPV.cshtml", Coupon);
            }
            return Content("");
        }
        [Authorize]
        public IActionResult Upsert(int Id)
        {
            CouponVM couponVM = new CouponVM();
            couponVM.Categories = _categeoryRepo.GetAll().ToList();
            if (Id!=0) //Edit
            {
                Coupon coupon = _couponRepo.Get(c=>c.Id==Id);
                foreach (Category cat in coupon.ApplicableCategories)
                {
                    couponVM.SelectedCatIds.Add(cat.Id);
                }
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
                    couponVM.DiscountValue = coupon.DiscountValue;
                    couponVM.MaxDiscountAmount = coupon.MaxDiscountAmount;
                    couponVM.MinPurchaseAmount = coupon.MinPurchaseAmount;
                    couponVM.ApplicableCategories = coupon.ApplicableCategories.ToList();
                }
            }

            return View(couponVM);
        }

        private void AddCatToCoupon(List<int>CatIds, Coupon Coupon)
        {
            if (!CatIds.IsNullOrEmpty())
            {
                Coupon.ApplicableCategories.Clear();

                foreach (int Id in CatIds)
                {
                    Category category = _categeoryRepo.Get(c => c.Id == Id);
                    if (category != null)
                    {
                        Coupon.ApplicableCategories.Add(category);
                    }
                }
                _couponRepo.SaveChanges();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Upsert(CouponVM couponVM)
        {
            if(ModelState.IsValid)
            {
                if (couponVM.ExpirationDate > DateTime.Now)
                {
                    couponVM.IsActive = true;
                }
                else
                {
                    couponVM.IsActive = false;
                }
                Coupon _coupon = new Coupon();
                AddCatToCoupon(couponVM.SelectedCatIds, _coupon);
                if (couponVM.Id == 0) //New Coupon
                {
                    _coupon.ExpirationDate = couponVM.ExpirationDate;
                    _coupon.UsageLimit = couponVM.UsageLimit;
                    _coupon.Code = couponVM.Code;
                    _coupon.Description = couponVM.Description;
                    _coupon.IsActive = couponVM.IsActive;
                    _coupon.IsSingleUse = couponVM.IsSingleUse;
                    _coupon.DiscountType = couponVM.DiscountType;
                    _coupon.MaxDiscountAmount = couponVM.MaxDiscountAmount;
                    _coupon.MinPurchaseAmount = couponVM.MinPurchaseAmount;
                    _couponRepo.add(_coupon);
                }
                else //Edit
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
                        coupon.DiscountValue = couponVM.DiscountValue;
                        coupon.MaxDiscountAmount = couponVM.MaxDiscountAmount;
                        coupon.MinPurchaseAmount = couponVM.MinPurchaseAmount;
                        AddCatToCoupon(couponVM.SelectedCatIds, coupon);
                    }

                }
                _couponRepo.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(couponVM);
        }
        [Authorize]
        [HttpDelete]
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

