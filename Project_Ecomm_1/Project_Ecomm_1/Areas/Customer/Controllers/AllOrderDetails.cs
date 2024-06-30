using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_1.DataAccess.Repository;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;
using Project_Ecomm_1.ViewModels;
using Stripe;
using Stripe.Issuing;
using System.Security.Claims;

namespace Project_Ecom_testOnee.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AllOrdersDetails : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        
        public AllOrdersDetails(IUnitofWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            
        }
        [BindProperty]
        public OrdersVM OrdersVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                OrdersVM = new OrdersVM()
                {
                    listOrder = new List<OrderDetail>(),
                    listOrderheader = new List<OrderHeader>()
                };

                return View(OrdersVM);
            }

            OrdersVM = new OrdersVM()
            {
                listOrderheader = _unitOfWork.OrderHeader.GetAll
    (sc => sc.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"),

                listOrder = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.ApplicationUserId == claim.Value, includeProperties: "Product,OrderHeader").OrderByDescending(u => u.Count)

            };

            foreach (var list in OrdersVM.listOrder)
            {
                list.Product = _unitOfWork.Product.FirstOrDefault(u => u.Id == list.ProductId, includeProperties: "Category,CoverType");
            }


            return View(OrdersVM);
        }

        public IActionResult OrderDetailsUser()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                OrdersVM = new OrdersVM()
                {
                    listOrder = new List<OrderDetail>(),
                    listOrderheader = new List<OrderHeader>()
                };

                return View(OrdersVM);
            }

            OrdersVM = new OrdersVM()
            {
                listOrderheader = _unitOfWork.OrderHeader.GetAll
                (sc => sc.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser"),

                listOrder = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.ApplicationUserId == claim.Value && u.OrderHeader.OrderStatus == SD.OrderStatusApproved, includeProperties: "Product,OrderHeader").OrderByDescending(u => u.Count),
                OrderHeader = new OrderHeader()
            };

            foreach (var list in OrdersVM.listOrder)
            {
                list.Product = _unitOfWork.Product.FirstOrDefault(u => u.Id == list.ProductId, includeProperties: "Category,CoverType");
            }

            foreach (var list in OrdersVM.listOrder)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrdersVM.OrderHeader.OrderTotal += (list.Price * list.Count);

            }

            return View(OrdersVM);
        }

        public IActionResult OrdersToCancel()
        {
          
            return View();
        }

        #region Api
        [HttpGet]
        [ActionName("GetAllApprove")]
        public IActionResult GetAllApproves()
        {
            var orders = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.OrderStatusApproved, includeProperties: "ApplicationUser");
            return Json(new { data = orders });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var orderInDb = _unitOfWork.OrderHeader.Get(id);
            if (orderInDb == null)
                return Json(new { success = false, message = "Something Went While Delete Data!!!" });
            _unitOfWork.OrderHeader.Remove(orderInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Data Successfully!!!" });
        }

        #endregion

        public IActionResult ForCancellations(int id)
        {
            return View(id);
        }

        [HttpPost]
        [ActionName("ForCancelAndRefunds")]
        public IActionResult ForCancelAndRefund(string stripeToken, int id)
        {
            StripeConfiguration.ApiKey = "pk_test_51OOeXTDAnK1BJoo1Y0e3eqBtNmZ6iz3vgkGHzouNTS1qnYfSM3dDaUW7KhNb56gcL43pMcF78PR4tyuvBgclakVG00sStyHCpg";

            var options = new RefundCreateOptions { PaymentIntent = "sk_test_51OOeXTDAnK1BJoo1OHvw1dDbxtib2RrOC448OlKkqhUBBuKeoDhU9hT0BFmjwz7qJV5K28AoVZqqIVhr2V4BT6jp00GGyC6oLZ" };
            var service = new RefundService();
            service.Create(options);

            return View();
        }

    }
}
