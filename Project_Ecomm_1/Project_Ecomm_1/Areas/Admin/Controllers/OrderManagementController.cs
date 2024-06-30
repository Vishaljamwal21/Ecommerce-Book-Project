using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;
using Project_Ecomm_1.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace Project_Ecom_testOnee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagerController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderManagerController(IUnitofWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        [BindProperty]
        public OrdersVM OrdersVM { get; set; }
        public IActionResult Index()
        {

            return View();
        }
        #region
        [HttpGet]
        public IActionResult GetAll()
        {
            var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = orderList });
        }
        [HttpGet]
        [ActionName("GetAllPendings")]
        public IActionResult GetAllPending()
        {
            var orders = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.OrderStatusPending, includeProperties: "ApplicationUser");
            //var pendingOrders = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.OrderStatus == );
            return Json(new { data = orders });
        }
        [HttpGet]
        [ActionName("GetAllApproveds")]
        public IActionResult GetAllApproved()
        {
            var orders = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.OrderStatusApproved, includeProperties: "ApplicationUser");
            return Json(new { data = orders });
        }
        #endregion
        public IActionResult Viewdata(int? id)
        {
            OrdersVM ordersVM = new OrdersVM()
            {

                OrderHeader = new OrderHeader(),
                listOrder = _unitOfWork.OrderDetail.GetAll
            (p => p.OrderHeaderId == id, includeProperties: "Product,OrderHeader")

            };

            var productindb = _unitOfWork.OrderDetail.GetAll
            (p => p.OrderHeaderId == id, includeProperties: "Product,OrderHeader");

            ordersVM.OrderHeader.OrderTotal = 0;

            foreach (var list in ordersVM.listOrder)
            {
                list.Product = _unitOfWork.Product.FirstOrDefault(p => p.Id == list.ProductId);
            }
            //var productinorders = _unitOfWork.Product.FirstOrDefault(p => p.Id == ordersVM.OrderDetail.ProductId);

            return View(ordersVM);


        }

        public IActionResult IndexForPending()
        {
            return View();
        }

        public IActionResult IndexForApproved()
        {

            return View();
        }

        public IActionResult IndexForCancelled()
        {

            return View();
        }
        public IActionResult DatewiseOrder()
        {

            var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            //return Json(new { data = orderList });

            return View(orderList);
        }


        public IActionResult DateWiseorderasp(DateTime startDate, DateTime endDate, string name)
        {
            if (name == "AllOrders")
            {
                var data = _unitOfWork.OrderHeader.GetAll().Where(d => d.OrderDate >= startDate && d.OrderDate <= endDate);
                return View(data);
            }
            if (name == "Pending")
            {
                var data = _unitOfWork.OrderHeader.GetAll().Where(d => d.OrderDate >= startDate && d.OrderDate <= endDate && d.OrderStatus == "pending");
                return View(data);
            }
            if (name == "Approved")
            {
                var data = _unitOfWork.OrderHeader.GetAll().Where(d => d.OrderDate >= startDate && d.OrderDate <= endDate && d.OrderStatus == "Approved");
                return View(data);
            }
            else
                return View(_unitOfWork.OrderHeader.GetAll().Where(d => d.OrderDate <= startDate && d.OrderDate >= endDate && d.OrderStatus == SD.OrderStatusApproved));
        }

    }
}
