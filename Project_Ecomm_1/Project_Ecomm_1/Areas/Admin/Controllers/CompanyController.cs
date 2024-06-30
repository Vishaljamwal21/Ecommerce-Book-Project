using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;

namespace Project_Ecomm_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CompanyController(IUnitofWork unitofwork) 
        {
            _unitofWork = unitofwork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new {data=_unitofWork.Company.GetAll()});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyInDb = _unitofWork.Company.Get(id);
            if (companyInDb == null)
                return Json(new { success = false , message = "Something Went Wrong While Delete Data"});
            _unitofWork.Company.Remove(companyInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Data Successfully Deleted" });
        }
        #endregion

        public IActionResult Upsert (int? id)
        {
            Company company=new Company();
            if(id==null) return View (company);
            company=_unitofWork.Company.Get(id.GetValueOrDefault());
            if(company==null)return NotFound();
            return View (company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert (Company company)
        {
            if (company==null) return BadRequest();
            if(!ModelState.IsValid)return View (company);
            if (company.Id==0)
                _unitofWork.Company.Add(company);
            else
                _unitofWork.Company.Update(company);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }


    
}
