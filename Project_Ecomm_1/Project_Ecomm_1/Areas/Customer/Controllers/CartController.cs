using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_1.DataAccess.Repository;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Models.ViewModels;
using Project_Ecomm_1.Utility;
using System.ComponentModel;
using System.Security.Claims;
using System.Linq;
using Stripe;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using static Project_Ecomm_1.Utility.TwillioService;

namespace Project_Ecomm_1.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private static bool IsEmailConfirm=false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        public CartController(IUnitofWork unitofwork,IEmailSender emailSender, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _unitofWork = unitofwork;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 100) + "...";
                }

            }
            //Email Settings
            if(!IsEmailConfirm)
            {
                ViewBag.EmailMessage = "Email must be confirm for authorize customer!";
                ViewBag.EmailCSS = "text-danger";
                IsEmailConfirm= false;

            }
            else
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email!!!";
                ViewBag.EmailCSS = "text-success";
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            if (user == null)
            
                ModelState.AddModelError(string.Empty, "Email Empty!!!");
            
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                IsEmailConfirm=true;

            }
            return RedirectToAction(nameof(Index));
        

        }
        public IActionResult plus(int id)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(x => x.Id == id);
            cart.Count += 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult mins(int id)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(x => x.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(x => x.Id == id);
            _unitofWork.ShoppingCart.Remove(cart);
            _unitofWork.Save();
            //****
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            //****
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary(string ids)
        {
            var climsIdentity = (ClaimsIdentity)User.Identity;
            var cliams = climsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == cliams.Value, includeProperties: "Product"),

            };
            if (ids == null)
            {


                ShoppingCartVM = new ShoppingCartVM()
                {

                    ListCart = _unitofWork.ShoppingCart.GetAll(Sc => Sc.ApplicationUserId == cliams.Value, includeProperties: "Product"),
                    OrderHeader = new OrderHeader()

                };
            }
            else
            {
                ShoppingCartVM = new ShoppingCartVM()
                {

                    ListCart = _unitofWork.ShoppingCart.GetAll(Sc => ids.Contains(Sc.Id.ToString()), includeProperties: "Product"),
                    OrderHeader = new OrderHeader()
                };
            }

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser
            .FirstOrDefault(au => au.Id == cliams.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity
                    (list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "..";
                }
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken, string ids)
        {


            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userlist = _unitofWork.ApplicationUser.FirstOrDefault(ul => ul.Id == claims.Value);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(u => u.Id == claims.Value);
            ShoppingCartVM.ListCart = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product").Where(x => ids.Contains(x.Id.ToString()));
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            //ShoppingCartVM.ListCart = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product");
            if (!string.IsNullOrEmpty(ids))
            {
                string[] itemIds = ids.Split(','); // Assuming IDs are comma-separated
                ShoppingCartVM.ListCart = _unitofWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == claims.Value && itemIds.Contains(sc.Id.ToString()), includeProperties: "Product");
            }
            else
            {
                ShoppingCartVM.ListCart = new List<ShoppingCart>(); // Create an empty list or handle this case as needed.
            }


            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
            _unitofWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofWork.Save();
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count,
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                _unitofWork.OrderDetail.Add(orderDetail);
                _unitofWork.Save();
            }
            _unitofWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitofWork.Save();
            ////Session
            //HttpContext.Session.Set("SelectesItems",selectItems);
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
            #region Stripe
            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Today.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description="OrderId:"+ShoppingCartVM.OrderHeader.Id,
                    Source=stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId==null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransectionId = charge.BalanceTransactionId;
                if(charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderDate =DateTime.Now;
                    var userId = await _userManager.GetUserIdAsync(userlist);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(userlist);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code },
                        protocol: Request.Scheme);


                    await _emailSender.SendEmailAsync(userlist.Email, "Order Is Confirmed",
                        $"Order has been confirmed.....Thanks for shopping with us... you will receive your package shortly........."
                        + ShoppingCartVM.OrderHeader.Id + $"Your Orderdate" + ShoppingCartVM.OrderHeader.OrderDate);

                }
                _unitofWork.Save();
            }

            #endregion
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id,DateTime orderdate)
        {
            var orderconfirm = _unitofWork.OrderHeader.Get(id);
            var twilioService = new TwilioService(_configuration);

            // Use the TwilioService to send messages, make calls, etc.
            twilioService.SendMessage("+919805101904", "+15856876023",
                $"Order has been confirmed.....Thanks for shopping with us... you will receive your package shortly........." +
                $"Your Ordernumber is:{id}" +
                $"Your OrderDate is:{orderdate}");

            return View(orderconfirm);
        }
    }
}