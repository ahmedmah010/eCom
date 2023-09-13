using eCom.DataAccess.Repos.IRepos;
using eCom.Models;
using eCom.Models.EntityTypeConfiguration;
using eCom.Models.ViewModels;
using eCom.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;


namespace eComApp.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
       


        /* Helper Methods */
        /* ////////////////////////////////////////////////////////////////////////////////////////////////////// */

        public void CatTagSelectList(ProductVM _vm)
        {
            _vm.CategoriesList = _unitOfWork.category.GetAll()
                .Select(c=> new SelectListItem {Text=c.Name, Value=c.Id.ToString()}).ToList();
            _vm.TagsList = _unitOfWork.tag.GetAll()
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() }).ToList();
        }
        public void SetSelectedProductTags(Product p, ProductVM _vm)
        {
            foreach (ProductTag pd in p.ProdTag)
            {
                _vm.SelectedTags.Add(pd.TagId.ToString());
            }
        }

        public bool CheckImages(List<IFormFile> images)
        {
            if (images.Count > 4 || images.Count<=0)
            {
                return false;
            }
            string[] allowedExtentions = { ".jpg", ".png", ".jpeg",".webp"};
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string imagesPath = Path.Combine(wwwrootPath, "images");
            if (Directory.Exists(imagesPath))
            {
                foreach (IFormFile image in images)
                {

                    if (!allowedExtentions.Contains(Path.GetExtension(image.FileName)))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        
        public void UploadImages(Product prod, List<IFormFile>images)
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string imagesPath = Path.Combine(wwwrootPath, "images");
            string ProdFolderPath = Path.Combine(imagesPath, $"Prod_Id_{prod.Id}");
            if (prod.Images==null)
            {
                Directory.CreateDirectory(ProdFolderPath);
            }
            foreach(IFormFile image in images)
            {
                string imageName = Guid.NewGuid().ToString() + image.FileName;
                using(var fs = new FileStream(Path.Combine(ProdFolderPath, imageName),FileMode.Create))
                {
                    image.CopyTo(fs);
                }
                ProductImage newImg = new ProductImage() { Name = imageName, ProductId = prod.Id };
                _unitOfWork.productImage.add(newImg);
            }
        }
        /* End of helper methods */
        /* ////////////////////////////////////////////////////////////////////////////////////////////////////// */

        public IActionResult Index()
        {
            return View(_unitOfWork.product.GetAll());
        }

        public IActionResult Upsert(int id) 
        {
            ProductVM _vm = new ProductVM();
            if (id > 0)
            {
                Product product = _unitOfWork.product.Get(p => p.Id == id);
                if (product != null)
                {
                    _vm.Title = product.Title;
                    _vm.Description = product.Description;
                    _vm.CurrentPrice = product.CurrentPrice;
                    _vm.OldPrice = product.OldPrice;
                    _vm.CurrentQuantity = product.CurrentQuantity;
                    _vm.Brand = product.Brand;
                    _vm.Discount = product.Discount;
                    _vm.Images = product.Images;
                    _vm.CategoryId = product.CategoryId;
                    _vm.SelectedTags = new List<string>();
                    _vm.Id = product.Id;
                    SetSelectedProductTags(product, _vm);
                    CatTagSelectList(_vm);
                    return View(_vm);
                }
            }
            else if (id == 0)
            {
                CatTagSelectList(_vm);
                return View(_vm);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM _vm, List<IFormFile> productImages)
        {
            if(_vm.Id==0 && !CheckImages(productImages))
            {
                ModelState.AddModelError("Images", "Something is wrong with your images, please try again");
            }
            if (ModelState.IsValid)
            {
                Product p;
                if (_vm.Id == 0) //add
                {
                    p = new Product
                    {
                        Title = _vm.Title,
                        Description = _vm.Description,
                        Brand = _vm.Brand,
                        CategoryId = _vm.CategoryId,
                        CurrentQuantity = _vm.CurrentQuantity,
                        OldPrice = _vm.OldPrice,
                        Discount = _vm.Discount,
                    };
                    _unitOfWork.product.add(p);
                    _unitOfWork.SaveChanges();
                    UploadImages(p, productImages);
                    foreach (string tag in _vm.SelectedTags)
                    {
                        ProductTag pt = new ProductTag { ProductId = p.Id, TagId = int.Parse(tag) };
                        _unitOfWork.productTag.add(pt);
                    }

                }
                else //update
                {
                    p = _unitOfWork.product.Get(p => p.Id == _vm.Id);
                    p.Title = _vm.Title;
                    p.Description = _vm.Description;
                    p.Brand = _vm.Brand;
                    p.CategoryId = _vm.CategoryId;
                    p.CurrentQuantity = _vm.CurrentQuantity;
                    p.CurrentPrice = _vm.CurrentPrice;
                    p.Discount = _vm.Discount;
                    //To skip unnecessary updates
                    if(productImages.Count==0)
                    {
                        List<string> tagids = p.Tags.Select(t => t.Id.ToString()).ToList();
                        if(tagids.SequenceEqual(_vm.SelectedTags))
                        {
                            if(!_unitOfWork.HasChanges())
                            {
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    if (productImages.Count != 0 && (productImages.Count + p.Images.Count) <= 4)
                    {
                        if (CheckImages(productImages))
                        {
                            UploadImages(p, productImages);
                        }
                        else
                        {
                            ModelState.AddModelError("Images", "Something is wrong with your images, please try again");
                            CatTagSelectList(_vm);
                            _vm.Images = _unitOfWork.product.Get(p => p.Id == _vm.Id).Images;
                            return View(_vm);
                        }
                    }
                    foreach (ProductTag pt in p.ProdTag)
                    {
                        _unitOfWork.productTag.remove(pt);
                    }
                    foreach (string tag in _vm.SelectedTags)
                    {
                        ProductTag pt = new ProductTag { ProductId = p.Id, TagId = int.Parse(tag) };
                        _unitOfWork.productTag.add(pt);
                    }
                }

                if (p.Discount != null) //apply discount
                {
                        
                    p.CurrentPrice = p.OldPrice - p.Discount.Value;
                }
                else
                {
                    p.CurrentPrice = p.OldPrice;
                }
                _unitOfWork.SaveChanges();
            
                return RedirectToAction("Index");
            }
            if(_vm.Id!=0)
            {
                _vm.Images = _unitOfWork.product.Get(p => p.Id == _vm.Id).Images;
            }
            CatTagSelectList(_vm);
            
            return View(_vm);
        }

        public IActionResult RemoveProduct(int id)
        {
            if(id!=0)
            {
                Product prod = _unitOfWork.product.Get(p=>p.Id==id);
                if(prod!=null)
                {
                    string ProdImagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", $"Prod_Id_{prod.Id}");
                    if(Directory.Exists(ProdImagesPath))
                    {
                        Directory.Delete(ProdImagesPath, true);
                    }
                    _unitOfWork.product.remove(prod);
                    _unitOfWork.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult ViewProduct(int?id)
        {
            if(id==null || id==0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Product prod = _unitOfWork.product.Get(p => p.Id == id);
                
                return View(prod);
            }
        }

        
        public IActionResult DeleteProductImage(int id)
        {
            if(id!=0) 
            {
                var image = _unitOfWork.productImage.Get(i=>i.Id==id);
                if(image!=null)
                {
                    var product = image.product;
                    string imgpath = Path.Combine(_webHostEnvironment.WebRootPath, "images", $"Prod_Id_{product.Id}", image.Name);
                    if(System.IO.File.Exists(imgpath))
                    {
                        System.IO.File.Delete(imgpath);
                    }
                    _unitOfWork.productImage.remove(image); 
                   _unitOfWork.SaveChanges();

                   return PartialView("_Productimages",product.Images);

                }
            }
            return BadRequest();

        }
        
    }
}
