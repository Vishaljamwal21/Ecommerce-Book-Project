using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.DataAccess.Repository.IRepository
{
    public interface IUnitofWork
    {
        public ICategoryRepository Category { get; }
        public ICoverTypeRepository CoverType { get; }
        public ISP_Call SP_Call { get; }
        public IProductRepository Product { get; }
        public ICompanyRepository Company { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailRepository OrderDetail { get; }
    

        void Save();
    }
}
