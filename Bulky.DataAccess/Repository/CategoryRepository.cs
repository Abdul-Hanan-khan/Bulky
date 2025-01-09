using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
			_db = db;
        }

        

		public List<Category> SearchCategory(string query)
		{
			List<Category> filteredCategories;

			if (query != "" && query != null)
			{
				filteredCategories = _db.Categories
					.Where(c => c.Name.Contains(query))
					.ToList();
			}
			else
			{
				filteredCategories = _db.Categories.ToList();
			}
			return filteredCategories;

		}

		public void Update(Category obj )
		{
			_db.Categories.Update(obj);
		}
	}
}
