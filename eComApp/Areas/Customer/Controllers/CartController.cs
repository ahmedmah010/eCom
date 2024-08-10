using Azure.Core.Serialization;
using eCom.DataAccess.Data;
using eCom.DataAccess.Migrations;
using eCom.Models;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text.Json.Nodes;
using System.Xml;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CartController(AppDbContext context, UserManager<AppUser>um)
        {
            _context = context; 
            _userManager = um;
        }

        private void UpdateCartItems(List<CartItemVM> cartItems)
        {
            CartItemVM.TotalPrice = 0;
            foreach (CartItemVM Item in cartItems)
            {
                Product p = _context.Products.Include(p=>p.Images).FirstOrDefault(p => p.Id == Item.ProductId);
                if (p != null)
                {
                    Item.Title = p.Title;
                    Item.Brand = p.Brand;
                    Item.ProductId = p.Id;
                    Item.Image = p.Images[0].Name;
                    Item.ProductPrice = p.CurrentPrice;
                    Item.SubTotal = Item.Qty * Item.ProductPrice;
                    CartItemVM.TotalPrice += (Item.SubTotal);
                }
            }
        }
        private List<CartItemVM> GetCartItemsFromCookie()
        {
            CartItemVM.TotalPrice = 0;
            List<CartItemVM> cartItems = new List<CartItemVM>();
            if (Request.Cookies["CartData"] != null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
                UpdateCartItems(cartItems);
            }
            return cartItems;
        }
        private async Task<List<CartItemVM>> GetCartItemsFromDB()
        {
            CartItemVM.TotalPrice = 0;
            List<CartItemVM> cartItemsVM = new List<CartItemVM >();
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            user = _context.Set<AppUser>().Include(u => u.CartItems).FirstOrDefault(u=>u.Id == user.Id);
            foreach (var item in user.CartItems)
            {
                Product prd = _context.Products.FirstOrDefault(prd => prd.Id == item.ProductId);
                if (prd != null)
                {
                    cartItemsVM.Add(new CartItemVM { ProductId = item.ProductId, Brand = prd.Brand, Image = prd.Images[0].Name, ProductPrice = prd.CurrentPrice, Qty = item.Qty, Title = prd.Title, SubTotal = item.Qty * prd.CurrentPrice });
                    CartItemVM.TotalPrice += (item.Qty*prd.CurrentPrice);
                    
                }
            }

            return cartItemsVM;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemVM> cartItemsVM;
            if (User.Identity.IsAuthenticated)
            {
                cartItemsVM = await GetCartItemsFromDB(); 
            }
            else
            {
                cartItemsVM = GetCartItemsFromCookie();
            }
            return View(cartItemsVM);
        }
        [Authorize]
        private void AddCartItemToDB(Product prd)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;
            UserCartItem? userCartItem = user.CartItems.FirstOrDefault(c => c.ProductId == prd.Id); //means prod already exists in the cart
            if (userCartItem!=null)
            {
                userCartItem.Qty += 1;
            }
            else
            {
                user.CartItems.Add(new UserCartItem { ProductId = prd.Id, Qty = 1 });
            }
            CartItemVM.TotalPrice += prd.CurrentPrice;
            _context.SaveChanges();
        }
        [Authorize]
        private void DeleteCartItemFromDB(int id)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;
            UserCartItem userCartItem = user.CartItems.FirstOrDefault(c=>c.ProductId==id);
            if (userCartItem != null)
            {
                CartItemVM.TotalPrice -= (userCartItem.Qty * _context.Products.Find(userCartItem.ProductId).CurrentPrice);
                _context.Remove(userCartItem);
                _context.SaveChanges();
            }
        }
        [Authorize]
        private void UpdateUserCartItemQuan(int id, int newQuan)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;
            user.CartItems.First(c=>c.ProductId==id).Qty = newQuan;
            _context.SaveChanges();
        }
        public IActionResult Add(int Id)
        {
            Product p = _context.Products.Find(Id);
            if (p != null) 
            {
                if (User.Identity.IsAuthenticated)
                {
                    AddCartItemToDB(p);
                }
                else
                {
                    List<CartItemVM> cartItems = new List<CartItemVM>();
                    CookieOptions Cookie = new CookieOptions();
                    Cookie.Expires = DateTime.Now.AddDays(15);
                    if (Request.Cookies["CartData"] != null)
                    {
                        cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
                    }
                    CartItemVM? cartItemVM = cartItems.FirstOrDefault(c=>c.ProductId==Id); //means prod already exists in cart cookie
                    if (cartItemVM!=null)
                    {
                        UpdateCartItemQuan(Id, cartItemVM.Qty+1);
                    }
                    else
                    {
                        CartItemVM cartItem = new CartItemVM();
                        cartItem.ProductId = p.Id;
                        cartItem.Qty = 1;
                        cartItems.Add(cartItem);
                        Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), Cookie);
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult UpdateCartItemQuan(int Id, int quan)
        {
            Product p = _context.Products.Find(Id);
            if (p.CurrentQuantity < quan)
            {
                TempData["QuanExceeded"] = Id.ToString();
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    UpdateUserCartItemQuan(Id, quan);
                }
                else
                {
                    List<CartItemVM> cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
                    cartItems.Find(ci => ci.ProductId == Id).Qty = quan;
                    Response.Cookies.Delete("CartData");
                    Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
                }
            }
            return RedirectToAction("CartItemsPV");
        }
        public async Task<IActionResult> CartItemsPV()
        {
            List<CartItemVM> cartItemsVM;
            if (User.Identity.IsAuthenticated)
            {
                cartItemsVM = await GetCartItemsFromDB();
            }
            else 
            {
                cartItemsVM = GetCartItemsFromCookie();
            }
            
            return PartialView("_CartItems", cartItemsVM);
        }
        public IActionResult DeleteCartItem(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                DeleteCartItemFromDB(id);
            }
            else
            {
                List<CartItemVM> cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
                CartItemVM target = cartItems.Find(x => x.ProductId == id);
                //Product p = _context.Products.FirstOrDefault(p => p.Id == target.ProductId);
                CartItemVM.TotalPrice -= (target.Qty * target.ProductPrice);
                cartItems.Remove(target);
                Response.Cookies.Delete("CartData");
                if (cartItems.Count != 0)
                {
                    Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> SaveCartTCookieToDB()
        {
            List<CartItemVM> cartItems = GetCartItemsFromCookie();
            if (!cartItems.IsNullOrEmpty()) //To avoid calling the same method more than one time
            {
                AppUser user = await _userManager.GetUserAsync(User);
                foreach (var item in cartItems)
                {
                    user.CartItems.Add(new UserCartItem { ProductId = item.ProductId, Qty = item.Qty, User = user});
                }
                _context.SaveChanges();
                TempData["SaveCartToDB"] = "1";
                Response.Cookies.Delete("CartData");
                cartItems.Clear();
            }
            return RedirectToAction("Index", "Cart");
        }

    }
}
