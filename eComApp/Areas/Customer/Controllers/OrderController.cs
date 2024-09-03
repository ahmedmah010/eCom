using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using eCom.Utilities.ExtentionMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class OrderController : Controller
    {
        private readonly IRepo<UserCartItem> _userCartItemRepo;
        private readonly IRepo<Tax> _taxRepo;
        private readonly IRepo<Order> _orderRepo;
        private readonly IRepo<Coupon> _couponRepo;
        private readonly IRepo<UserAddress> _addressRepo;
        private readonly IRepo<Product> _productRepo;
        private readonly UserManager<AppUser>  _userManager;
        public OrderController
            (
                               IRepo<UserCartItem> userCartItemRepo, 
                               IRepo<Tax> taxRepo, 
                               IRepo<Order> orderRepo,
                               IRepo<Coupon> couponRepo,
                               IRepo<UserAddress> addressRepo,
                               IRepo<Product> productRepo,
                               UserManager<AppUser> userManager
            )
        {
            _userCartItemRepo = userCartItemRepo;
            _taxRepo = taxRepo;
            _orderRepo = orderRepo;
            _couponRepo = couponRepo;
            _addressRepo = addressRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (_userCartItemRepo.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            OrderVM orderVM = new OrderVM();
            orderVM.Addresses = _addressRepo.Where(add=>add.UserId == _userManager.GetUserId(User)).ToList();
            List<UserCartItem> cartItems = _userCartItemRepo.Where(item => item.UserId == _userManager.GetUserId(User)).ToList();
            HttpContext.Session.SetObjectToJson("cartItems",cartItems); //this is a helper method. 
            foreach (var cartItem in cartItems)
            {
                Product prod = _productRepo.Get(p=>p.Id == cartItem.ProductId);

                orderVM.OrderItemsVM.Add(new OrderItemVM { ProductId = cartItem.ProductId, Price = prod.CurrentPrice, ProductBrand = prod.Brand, ProductImage = prod.Images[0].Name, ProductTitle = prod.Title, Quantity = cartItem.Qty });
            }
            return View(orderVM);
        }
        private float CartTotalPrice()
        {
            float totalPrice = 0;
            List<UserCartItem> cartItems = HttpContext.Session.GetObjectFromJson<List<UserCartItem>>("cartItems");
            //_userCartItemRepo.Where(item => item.UserId == _userManager.GetUserId(User)).ToList();
            foreach (var cartItem in cartItems)
            {
                Product prod = _productRepo.Get(p => p.Id == cartItem.ProductId);
                totalPrice += prod.CurrentPrice * cartItem.Qty;
            }
            return totalPrice;
        }
        public IActionResult OrderSummaryPV(OrderVM orderVM)
        {
            if(orderVM.PaymentMethod != null && orderVM.ChosenAddressId != 0)
            {
                OrderSummaryVM orderSummaryVM = new OrderSummaryVM();
                orderSummaryVM.DeliveryFees = _addressRepo.Get(add=>add.Id==orderVM.ChosenAddressId).City.DeliveryFee;
                orderSummaryVM.TotalPriceBefore = CartTotalPrice();
                if(orderVM.PaymentMethod == PaymentMethod.COD)
                {
                    Tax tax = _taxRepo.Get(tax => tax.Name == "COD");
                    orderSummaryVM.AppliedTaxes.Add(tax);
                    orderSummaryVM.TotalPriceAfter += tax.Amount;
                }
                foreach(var tax in _taxRepo.GetAll().ToList())
                {
                    if (tax.Name != "COD")
                    {
                        orderSummaryVM.AppliedTaxes.Add(tax);
                        if (tax.TaxType == TaxType.Percentage)
                        {
                            orderSummaryVM.TotalPriceAfter += (float)(tax.Amount/100.0) * orderSummaryVM.TotalPriceBefore;
                        }
                        else
                        {
                            orderSummaryVM.TotalPriceAfter += tax.Amount;
                        }
                       
                    }
                }
                if (orderVM.Coupon != null)
                {
                    Coupon coupon = _couponRepo.Get(coupon => coupon.Code == orderVM.Coupon);
                    if (coupon != null)
                    {
                        if(coupon.DiscountType == DiscountType.Percentage)
                        {
                            orderSummaryVM.Discount = orderSummaryVM.TotalPriceBefore * (coupon.DiscountValue/100);
                        }
                        else
                        {
                            orderSummaryVM.Discount = coupon.DiscountValue;
                        }
                    }
                }
                orderSummaryVM.TotalPriceAfter += orderSummaryVM.TotalPriceBefore - orderSummaryVM.Discount + orderSummaryVM.DeliveryFees;
                return PartialView("~/Areas/Customer/Views/Order/PartialViews/OrderSummaryPV.cshtml", orderSummaryVM);
            }
            return Content("");
            
        }
        private bool IsCouponValid(string code)
        {
            Coupon coupon = _couponRepo.Get(c=>c.Code == code);
            if (coupon != null)
            {
                HashSet<UserCartItem> carItems = HttpContext.Session.GetObjectFromJson<HashSet<UserCartItem>>("cartItems");
                bool validForCategory = false, validForProduct = false;
                float subTotal = 0;
                foreach (UserCartItem item in carItems)
                {
                    Product p = _productRepo.Get(p => p.Id == item.ProductId);
                    subTotal += (p.CurrentPrice * item.Qty);
                    if (!validForCategory)
                    {
                        validForCategory = coupon.ApplicableCategories.Any(cat=>cat==p.Category);
                     
                    }
                    if(!validForProduct)
                    {
                        validForProduct = coupon.ApplicableProducts.Any(prod => prod == p);
                    }
                }
                if(coupon.ExpirationDate>=DateTime.Now && coupon.MinPurchaseAmount<=subTotal && (validForProduct || validForCategory))
                {  return true; }
            }
            return false;
        }
        public IActionResult ValidateCoupon(string code)
        {
            bool Res = IsCouponValid(code);
            if (Res)
            {
                return Content("true");
            }
            return Content("false");
        }

    }
}
