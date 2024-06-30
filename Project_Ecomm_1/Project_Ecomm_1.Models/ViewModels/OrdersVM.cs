using Project_Ecomm_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.ViewModels
{
    public class OrdersVM
    {
        //public IEnumerable<ShoppingCart> listCarts { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> listOrder { get; set; }
        public IEnumerable<OrderHeader> listOrderheader { get; set; }

    }


}
