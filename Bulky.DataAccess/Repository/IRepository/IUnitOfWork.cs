﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository CategoryRepo { get; }
		IProductRepository ProductRepo { get; }
		ICompanyRepository CompanyRepo{ get; }
		IShoppingCartRepository ShoppingCartRepo{ get; }
		IApplicationUserRepository ApplicationUserRepo{ get; }
		IOrderDetailRepository OrderDetailRepo{ get; }
		IOrderHeaderRepository OrderHeaderRepo{ get; }
		void Save();
	}
}
