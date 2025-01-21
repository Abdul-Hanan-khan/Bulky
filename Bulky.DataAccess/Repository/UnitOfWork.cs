using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ApplicationDbContext _db;
		public ICategoryRepository CategoryRepo { get; private set; }
		public IProductRepository ProductRepo { get; private set; }
		public ICompanyRepository CompanyRepo{ get; private set; }
		public IShoppingCartRepository ShoppingCartRepo{ get; private set; }
		public IApplicationUserRepository  ApplicationUserRepo{ get; private set; }
		public IOrderDetailRepository OrderDetailRepo{ get; private set; }
		public IOrderHeaderRepository OrderHeaderRepo { get; private set; }
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			CategoryRepo = new CategoryRepository(_db);
			ProductRepo = new ProductRepository(_db);
			CompanyRepo = new CompanyRepository(_db);
			ShoppingCartRepo = new ShoppingCartRepository(_db);
			ApplicationUserRepo = new ApplicationUserRepository(_db);
			OrderDetailRepo = new OrderDetailRepository(_db);
			OrderHeaderRepo = new OrderHeaderRepository(db);



		}
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
