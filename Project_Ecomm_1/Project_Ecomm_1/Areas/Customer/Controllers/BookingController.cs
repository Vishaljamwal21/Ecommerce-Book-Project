using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models.ViewModels;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;
using System.Security.Claims;

namespace Project_Ecomm_1.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        public BookingController(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);

            var bookinglist = _unitOfWork.OrderHeader.GetAll(sc => sc.ApplicationUserId == claim.Value);
            return View(bookinglist);
           
        }
        //[HttpPost]
        //public IActionResult DuplicateOrder(int Id)
        //{
        //    var ordertoduplicate = _unitOfWork.OrderDetails.GetAll(c => c.Id == Id);
        //    if(ordertoduplicate ==null) return NotFound();

        //    //var shoppingCart = new ShoppingCart();
        //    foreach (var order in ordertoduplicate)
        //    {
        //        var orderHeader = _unitOfWork.OrderHeader.Get(order.OrderHeaderId);
        //        var claimidentity = (ClaimsIdentity)User.Identity;
        //        var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
        //        var product = _unitOfWork.Product.Get(order.ProductId); // Assuming you have a method to get a product by Id
        //        if (product == null)
        //        {
        //            // Handle the case where the product is not found
        //            return NotFound();
        //        }

        //        var shoppingCartItem = new ShoppingCart
        //        {
        //            //ApplicationUserId =orderHeader.ApplicationUserId,
        //            ProductId = order.ProductId,
        //            Count = order.Count,
        //            Price = product.Price // Set the price based on your specific logic
        //        };

        //        _unitOfWork.ShoppingCart.Add(shoppingCartItem);
        //        _unitOfWork.Save();
        //    }

        //    return RedirectToAction("index", "Cart");
        //}


        [HttpPost]
        public IActionResult DuplicateOrder(int Id)
        {
            try
            {
                var ordertoduplicate = _unitOfWork.OrderDetail.GetAll(c => c.OrderHeaderId == Id);
                if (ordertoduplicate == null) return NotFound();

                foreach (var order in ordertoduplicate)
                {
                    var orderHeader = _unitOfWork.OrderHeader.Get(order.OrderHeaderId);
                    var product = _unitOfWork.Product.Get(order.ProductId);

                    if (product == null || orderHeader == null)
                    {
                        // Handle the case where the product or order header is not found
                        return NotFound();
                    }
                    var shoppingCartItem = new ShoppingCart
                    {
                        ApplicationUserId = orderHeader.ApplicationUserId,
                        ProductId = order.ProductId,
                        Count = order.Count,
                        Price = product.Price
                    };
                    _unitOfWork.ShoppingCart.Add(shoppingCartItem);
                }
                _unitOfWork.Save(); // Save changes to the database
                var claimidentity = (ClaimsIdentity)User.Identity;
                var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                    HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
                }
                return RedirectToAction("index", "Cart");
            }
            catch (Exception ex)
            {
                // Log or print the exception details
                Console.WriteLine(ex.Message);
                return StatusCode(500); // Internal Server Error
            }
        }
    }
}
