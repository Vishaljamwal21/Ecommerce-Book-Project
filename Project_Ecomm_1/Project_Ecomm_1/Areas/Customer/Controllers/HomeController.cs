using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecomm_1.DataAccess.Repository;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Models.ViewModels;
using Project_Ecomm_1.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_Ecomm_1.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork _unitofWork;


        public HomeController(ILogger<HomeController> logger, IUnitofWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        public IActionResult Index(string selectedOption, string searchTerm)

        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim !=null)
            {
                var Count = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, Count);
            }

                var productList = _unitofWork.Product.GetAll(includeProperties: "Category,CoverType");
            // Dropdown list options
            List<string> searchOptions = new List<string>
            {
                "Title",
                "Author"
            };

            // Pass the search options and fetched data to the view
            ViewBag.SearchOptions = new SelectList(searchOptions);
            if (string.IsNullOrEmpty(selectedOption) && string.IsNullOrEmpty(searchTerm))
            {
                return View(productList);
            }

            // Filter the data based on the selected search option and search term
            if (!string.IsNullOrEmpty(selectedOption) && !string.IsNullOrEmpty(searchTerm))
            {
                if (selectedOption == "Title")
                {
                    productList = productList.Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (selectedOption == "Author")
                {
                    productList = productList.Where(p => p.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            // Pass the filtered data as the model to the view
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var Count = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, Count);
            }
            var productInDb=_unitofWork.Product.FirstOrDefault(x=>x.Id== id,includeProperties:"Category,CoverType");
            if (productInDb == null) return NotFound();
            var ShoppingCart = new ShoppingCart()
            {
                ProductId = id,
                Product = productInDb
            };
            return View(ShoppingCart);

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shoppingCart.ApplicationUserId = claim.Value;
                var shoppingCartFromDb=_unitofWork.ShoppingCart.FirstOrDefault(sc=>sc.ApplicationUserId==claim.Value && sc.ProductId==shoppingCart.ProductId);
                if (shoppingCartFromDb == null)
                    _unitofWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartFromDb.Count += shoppingCart.Count;
                _unitofWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitofWork.Product.FirstOrDefault(x => x.Id == shoppingCart.Id, includeProperties: "Category,CoverType");
                if (productInDb == null) return NotFound();
                var ShoppingCartedit = new ShoppingCart()
                {
                    ProductId =shoppingCart.Id,
                    Product = productInDb
                };
                return View(ShoppingCartedit);

            }
        }
    }
}
