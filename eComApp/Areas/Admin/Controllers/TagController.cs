using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace eComApp.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class TagController : Controller
    {
        private readonly IRepo<Tag> _repo;
        public TagController(IRepo<Tag> repo)
        {
                _repo = repo;
        }
        public IActionResult Index()
        {
            return View(_repo.GetAll());
        }
        public IActionResult Upsert(int? id)
        {
            if(id==null||id==0)
            {
                return View();
            }
            return View(_repo.Get(t=>t.Id==id));
        }
        [HttpPost]
        public IActionResult Upsert(Tag tag)
        {
            if (tag.Id == 0)
            {
                _repo.add(tag);
                _repo.SaveChanges();
            }
            //update
            else
            {
                _repo.update(tag);
                _repo.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int?id)
        {
            if(id!=0 || id!=null)
            {
                _repo.remove(_repo.Get(t=>t.Id==id));
                _repo.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
