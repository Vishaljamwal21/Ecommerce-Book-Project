using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Ecomm_1.DataAccess.Repository;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Models.ViewModels;
using Project_Ecomm_1.Utility;
using System.Data;

namespace Project_Ecomm_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;   
        public ProductController(IUnitofWork unitofWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = unitofWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitofWork.Product.Get(id);
            if(productInDb==null)
                return Json(new {success= false,message="Something Went While Delete Data!!!"});
            var webRootPath=_webHostEnvironment.WebRootPath;
            var imagePath=Path.Combine(webRootPath,productInDb.ImageUrl.Trim('\\'));
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitofWork.Product.Remove(productInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Delete Data Successfully!!!" });
        }
        #endregion
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitofWork.Category.GetAll().Select(
                    cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()

                    }),
                CoverTypeList = _unitofWork.CoverType.GetAll().Select(
                    cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                Product = new Product()
            };
            if (Id == null) return View(productVM);//Create
            //Edit
            productVM.Product = _unitofWork.Product.Get(Id.GetValueOrDefault());
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert (ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if(files.Count()>0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images\Products");
                    if(productVM.Product.Id !=0)
                    {
                        var imageExists = _unitofWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                    if (productVM.Product.ImageUrl !=null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\Products\" + fileName + extension;

                }
                else
                {
                    if (productVM.Product.Id !=0)
                    {
                        var imageExists = _unitofWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                }
                if (productVM.Product.Id == 0)
                    _unitofWork.Product.Add(productVM.Product);
                else
                    _unitofWork.Product.Update(productVM.Product);
                _unitofWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()
                {
                    CategoryList = _unitofWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitofWork.CoverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    Product = new Product()
                };
                if(productVM.Product.Id !=0)
                {
                    productVM.Product = _unitofWork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }
    }

}
