using Project_Ecomm_1.DataAccess.Data;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.DataAccess.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _context;
        public UnitofWork (ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            CoverType= new CoverTypeRepository(context);
            SP_Call = new SP_Call(context);
            Product = new ProductRepository(context);
            ApplicationUser=new ApplicationUserRepository(context);
            Company=new CompanyRepository(context);
            ShoppingCart=new ShoppingCartRepository(context);
            OrderHeader=new OrderHeaderRepository(context);
            OrderDetail=new OrderDetailRepository(context);
          
        }

        public ICategoryRepository Category { private set; get; }

        public ICoverTypeRepository CoverType { private set; get; }

        public ISP_Call SP_Call { private set; get; }

        public IProductRepository Product { private set; get; }
        public IApplicationUserRepository ApplicationUser { private set; get; }  
        public ICompanyRepository Company { private set; get; }
        public IShoppingCartRepository ShoppingCart { private set; get; }
        public IOrderHeaderRepository OrderHeader { private set; get; }
        public IOrderDetailRepository OrderDetail { private set; get; }


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
