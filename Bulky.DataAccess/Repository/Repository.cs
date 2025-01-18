﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Migrations;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
			_db = db;
			dbSet =  _db.Set<T>();
        }
        void IRepository<T>.Add(T entity)
		{
			dbSet.Add(entity);
		}

		T IRepository<T>.Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{
            IQueryable<T> query = dbSet;

			if (!tracked)
			{
                 query = dbSet.AsNoTracking();
            }
				
				if (!string.IsNullOrEmpty(includeProperties))
				{
					foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						query = query.Include(includeProp);
					}
				}
				query = query.Where(filter);
				return query.FirstOrDefault();
			
			

		}

		IEnumerable<T> IRepository<T>.GetAll(string ? includeProperties =null)  
		{
            IQueryable<T> query = dbSet;

			if (!string.IsNullOrEmpty(includeProperties)) {
                foreach (var includeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries) )
                {
                    query = query.Include(includeProp);
                }
            }

			return query.ToList();
		}

		void IRepository<T>.Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		void IRepository<T>.RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
