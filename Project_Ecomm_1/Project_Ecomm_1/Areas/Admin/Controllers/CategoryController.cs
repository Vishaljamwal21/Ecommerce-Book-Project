using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;

namespace Project_Ecomm_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CategoryController(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
           var categoryList= _unitofWork.Category.GetAll();
            return Json(new {data=categoryList});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitofWork.Category.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Something Went Wrong While Delete Data!!!" });
            _unitofWork.Category.Remove(categoryInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully!!" });
        }

        #endregion
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if(id == null) return View(category);
            category= _unitofWork.Category.Get(id.GetValueOrDefault());
            if(category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert (Category category)
        {
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View(category);
            DynamicParameters Param=new DynamicParameters();
            Param.Add("name", category.Name);
            if (category.Id == 0)
                 _unitofWork.Category.Add(category);
            else
                _unitofWork.Category.Update(category);
             _unitofWork.Save();
            return RedirectToAction("Index");
        }
    }
}
