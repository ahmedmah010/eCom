using Azure.Core.Serialization;
using eCom.DataAccess.Data;
using eCom.DataAccess.Migrations;
using eCom.Models;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace eComApp.Areas.Customer.Controllers
{
    [Area(Area.Customer)]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context; 
        }

        private void UpdateCartItems(List<CartItem> cartItems)
        {
            CartItem.TotalPrice = 0;
            foreach (CartItem Item in cartItems)
            {
                Product p = _context.Products.FirstOrDefault(p => p.Id == Item.ProductId);
                if (p != null)
                {
                    Item.Title = p.Title;
                    Item.Brand = p.Brand;
                    Item.ProductId = p.Id;
                    Item.Image = p.Images[0].Name;
                    Item.ProductPrice = p.CurrentPrice;
                    Item.SubTotal = Item.Qty * Item.ProductPrice;
                    CartItem.TotalPrice += (Item.SubTotal);
                }
            }
        }

        public IActionResult Index()
        {
            List<CartItem> cartItems = new List<CartItem>();
            if (Request.Cookies["CartData"]!=null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
                UpdateCartItems(cartItems);
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
                if (cartItems.Any(ci => ci.ProductId == Id))
                {
                    UpdateCartItemQuan(Id, cartItems.First(ci => ci.ProductId == Id).Qty + 1);
                }
                else
                {
                    CartItem cartItem = new CartItem();
                    cartItem.ProductId = p.Id;
                    cartItem.Qty = 1;
                    cartItems.Add(cartItem);
                    Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), Cookie);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult UpdateCartItemQuan(int Id, int quan)
        {
            List<CartItem> cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
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
            List<CartItem> cartItems = new List<CartItem>();
            if (Request.Cookies["CartData"] != null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
                UpdateCartItems(cartItems);
            }

            return PartialView("_CartItems",cartItems);
            
        }
        public IActionResult DeleteCartItem(int id)
        {
            List<CartItem> cartItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["CartData"]);
            CartItem target = cartItems.Find(x => x.ProductId == id);
            Product p = _context.Products.FirstOrDefault(p => p.Id == target.ProductId);
            CartItem.TotalPrice -= (target.Qty*p.CurrentPrice);
            cartItems.Remove(target);
            Response.Cookies.Delete("CartData");
            if (cartItems.Count != 0)
            {
                Response.Cookies.Append("CartData", JsonConvert.SerializeObject(cartItems), new CookieOptions { Expires = DateTime.Now.AddDays(15) });
            }
            return RedirectToAction("Index");
        }

    }
}
