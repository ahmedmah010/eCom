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
            List<CartItemVM> cartItemsVM = new List<CartItemVM >();
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            user = _context.Set<AppUser>().Include(u => u.CartItems).FirstOrDefault(u=>u.Id == user.Id);
            foreach (var item in user.CartItems)
            {
                Product prd = _context.Products.FirstOrDefault(prd => prd.Id == item.ProductId);
                if (prd != null)
                {
                    cartItemsVM.Add(new CartItemVM { ProductId = item.ProductId, Brand = prd.Brand, Image = prd.Images[0].Name, ProductPrice = prd.CurrentPrice, Qty = item.Qty, Title = prd.Title, SubTotal = item.Qty * prd.CurrentPrice });
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
        private void AddCartItemToDB(Product prd)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;
            user = _context.Set<AppUser>().Include(u=>u.CartItems).FirstOrDefault(u=>u.Id==user.Id);
            user.CartItems.Add(new UserCartItem { ProductId = prd.Id, Qty = 1});
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
                    if (cartItems.Any(ci => ci.ProductId == Id))
                    {
                        UpdateCartItemQuan(Id, 1);
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
            List<CartItemVM> cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
            Product p = _context.Products.Find(Id);
            if (p.CurrentQuantity >= quan)
            {
                cartItems.Find(ci => ci.ProductId == Id).Qty = quan;
                Response.Cookies.Delete("CartData");
                Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
            }
            else
            {
                TempData["QuanExceeded"] = Id.ToString();
            }
            return RedirectToAction("CartItemsPV");
        }
        public IActionResult CartItemsPV()
        {
            return PartialView("_CartItems", GetCartItemsFromCookie());
        }
        public IActionResult DeleteCartItem(int id)
        {
            List<CartItemVM> cartItems = JsonConvert.DeserializeObject<List<CartItemVM>>(Request.Cookies["CartData"]);
            CartItemVM target = cartItems.Find(x => x.ProductId == id);
            Product p = _context.Products.FirstOrDefault(p => p.Id == target.ProductId);
            CartItemVM.TotalPrice -= (target.Qty*p.CurrentPrice);
            cartItems.Remove(target);
            Response.Cookies.Delete("CartData");
            if (cartItems.Count != 0)
            {
                Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> SaveCartTCookieToDB()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<CartItemVM> cartItems = GetCartItemsFromCookie();
                AppUser user = await _userManager.GetUserAsync(User);
                foreach (var item in cartItems)
                {
                    user.CartItems.Add(new UserCartItem { ProductId = item.ProductId, Qty = item.Qty, User = user});
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Cart", "Index");
        }

    }
}
