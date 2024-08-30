using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
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
            foreach(var cartItem in _userCartItemRepo.GetAll().ToList())
            {
                Product prod = _productRepo.Get(p=>p.Id == cartItem.ProductId);

                orderVM.OrderItemsVM.Add(new OrderItemVM { ProductId = cartItem.ProductId, Price = prod.CurrentPrice, ProductBrand = prod.Brand, ProductImage = prod.Images[0].Name, ProductTitle = prod.Title, Quantity = cartItem.Qty });
            }
            return View(orderVM);
        }
    }
}
