using Azure.Core.Serialization;
using eCom.DataAccess.Data;
using eCom.DataAccess.Migrations;
using eCom.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace eComApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            List<CartItem> cartItems = new List<CartItem>();
            if (Request.Cookies["CartData"]!=null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
            }
   
            return View(cartItems);
        }
        public IActionResult Add(int Id)
        {
            Product p = _context.Products.Find(Id);
            if (p != null) 
            {
                List<CartItem> cartItems = new List<CartItem>();
                CookieOptions Cookie = new CookieOptions();
                Cookie.Expires = DateTime.Now.AddDays(15);
                if (Request.Cookies["CartData"]!=null)
                {
                    cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
                }
                CartItem cartItem = new CartItem();
                cartItem.ProductId = p.Id;
                cartItem.Qty = 1;
                cartItem.Brand = p.Brand;
                cartItem.Title = p.Title;
                cartItem.ProductPrice = p.CurrentPrice;
                cartItem.SubTotal = cartItem.Qty * cartItem.ProductPrice;
                cartItem.Img = p.Images[0].Name;
                cartItems.Add(cartItem);
                CartItem.TotalPrice += cartItem.SubTotal;
                Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), Cookie);
            }
            return RedirectToAction("Index");
        }

        public IActionResult CartItemPV(int id, int quan)
        {
            List<CartItem> cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
            CartItem target = cartItems.Find(x => x.Id == id);
            CartItem.TotalPrice -= target.SubTotal;
            target.Qty = quan;
            target.SubTotal = target.Qty * target.ProductPrice;
            CartItem.TotalPrice += target.SubTotal;
            Response.Cookies.Delete("CartData");
            Response.Cookies.Append("CartData",JsonConvert.SerializeObject(cartItems),new CookieOptions { Expires=DateTime.Now.AddDays(15)});

            
            return PartialView("_CartItem", target);
        }
        public IActionResult CartTotalPrice()
        {
            return PartialView("_CartTotalPrice");
        }
        public IActionResult DeleteCartItem(int id)
        {
            List<CartItem> cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
            CartItem target = cartItems.Find(x => x.Id == id);
            CartItem.TotalPrice = CartItem.TotalPrice == 0?0: CartItem.TotalPrice - target.SubTotal;
            cartItems.Remove(target);
            Response.Cookies.Delete("CartData");
            Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
            return RedirectToAction("Index");
        }

    }
}
