using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class TaxController : Controller
    {
        private readonly IRepo<Tax> _taxRepo;
        public TaxController(IRepo<Tax> taxRepo)
        {
            _taxRepo = taxRepo;
        }
        public IActionResult Index()
        {
            return View(_taxRepo.GetAll().ToList());
        }
        [HttpGet]
        public IActionResult Upsert(int Id)
        {
            Tax tax = new Tax();
            if (Id == 0)
            {
                return View(tax);
            }
            tax = _taxRepo.Get(tax => tax.Id == Id);
            if (tax != null)
            {
                return View(tax);
            }
            ViewBag.InvalidId = "Invalid Id";
            tax.Id = 0; //UI purpose, cuz If I entered an invalid Id, the title on that view will be "Update Tax" as the id is not 0 but it's still invalid so I'm assigning it to zero again
            return View(tax);
        }
        [HttpPost]
        public IActionResult Upsert(Tax tax)
        {
            if (ModelState.IsValid)
            {
                if (tax.Id == 0)
                {
                    _taxRepo.add(tax);
                }
                else
                {
                    _taxRepo.update(tax);
                }
                _taxRepo.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tax);
        }
        public IActionResult Delete(int id)
        {
            if (id != 0)
            {
                Tax tax = _taxRepo.Get(tax => tax.Id == id);
                if (tax != null)
                {
                    _taxRepo.remove(tax);
                    _taxRepo.SaveChanges();
                    TempData["DeleteTax"] = "Deleted Successfully!";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
