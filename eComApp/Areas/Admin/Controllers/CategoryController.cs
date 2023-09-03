using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepo<Category> _repo;
        public CategoryController(IRepo<Category> repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _repo.GetAll();
            return View(categories);
        }

        public ActionResult Upsert(int? id)
        {
            if (id == null || id==0) //add view
            {
                return View();
            }
            return View(_repo.Get(c=>c.Id==id)); //update view
        }
        [HttpPost]
        public ActionResult Upsert(Category cat)
        {
            if(ModelState.IsValid)
            {
                if(cat.Id==0)
                {
                    _repo.add(cat);
                    _repo.SaveChanges();
                }
                else
                {
                    _repo.update(cat);
                    _repo.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(cat);
            }
        }

        
        public ActionResult Delete(int? id)
        {
            if(id != null && id!=0)
            {
                _repo.remove(_repo.Get(c => c.Id == id));
                _repo.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
