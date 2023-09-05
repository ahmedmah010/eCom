using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
