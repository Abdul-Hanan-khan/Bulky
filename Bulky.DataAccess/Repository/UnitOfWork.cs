using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ApplicationDbContext _db;
		public ICategoryRepository CategoryRepo { get; private set; }
		public IProductRepository ProductRepo { get; private set; }
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			CategoryRepo = new CategoryRepository(_db);
			ProductRepo = new ProductRepository(_db);

		}
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
