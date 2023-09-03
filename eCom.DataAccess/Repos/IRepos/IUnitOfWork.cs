using eCom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Repos.IRepos
{
    public interface IUnitOfWork
    {
        IRepo<Product> product { get; set; }
        IRepo<Category> category { get; set; }
        IRepo<Tag> tag { get; set; }
        IRepo<ProductTag> productTag { get; set; }
        IRepo<ProductImage> productImage { get; set; }
        void SaveChanges();
        bool HasChanges();
    }
}
