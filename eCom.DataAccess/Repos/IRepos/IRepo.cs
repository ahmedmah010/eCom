﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eCom.DataAccess.Repos.IRepos
{
    public interface IRepo<T> where T : class
    {
        T Get(Expression<Func<T,bool>> match);
        T Get(Expression<Func<T, bool>> match, params string[] tables);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params string[]tables);
        void add(T item);
        void remove(T item);
        void update(T item);
        EntityEntry<T> GetEntity(T item);
        void SaveChanges();


    }
}