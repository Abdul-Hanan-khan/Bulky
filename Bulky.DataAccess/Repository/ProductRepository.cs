using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public List<Product> SearchProduct(string query)
        {
            List<Product> filteredProducts;

            if (query != "" && query != null)
            {
                filteredProducts = _db.Products
                    .Where(c => c.Title.Contains(query))
                    .ToList();
            }
            else
            {
                filteredProducts = _db.Products.ToList();
            }
            return filteredProducts;
        }

        public void Update(Product product)
        {
            _db.Update(product);
        }
    }
}
