using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;

namespace Project_Ecomm_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CoverTypeController(IUnitofWork unitofWork)
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
            //return Json(new {data =_unitofWork.CoverType.GetAll()});
            return Json(new { data = _unitofWork.SP_Call.List<CoverType>(SD.Proc_GetCoverTypes) });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitofWork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Something Went wrong!!!" });
            //_unitofWork.CoverType.Remove(coverTypeInDb);
            //_unitofWork.Save();
            DynamicParameters Param=new DynamicParameters();
            Param.Add("id", id);
            _unitofWork.SP_Call.Execute(SD.Proc_DeleteCoverTypes, Param);
            return Json(new { success = true, message = "Data Deleted Successfully!!!" });

        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType);
            // coverType =_unitofWork.CoverType.Get(id.GetValueOrDefault());
            DynamicParameters Param = new DynamicParameters();
            Param.Add("id", id.GetValueOrDefault());
            coverType = _unitofWork.SP_Call.oneRecord<CoverType>(SD.Proc_GetCoverType, Param);
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return BadRequest();
            if (!ModelState.IsValid) return View(coverType);
            DynamicParameters Param=new DynamicParameters();
            Param.Add("name", coverType.Name);
            if (coverType.Id == 0)
                //_unitofWork.CoverType.Add(coverType);
                _unitofWork.SP_Call.Execute(SD.Proc_CreateCoverTypes, Param);
            else
            {
                Param.Add("id", coverType.Id);
                _unitofWork.SP_Call.Execute(SD.Proc_UpdateCoverTypes, Param);
            }
                //_unitofWork.CoverType.Update(coverType);
            //_unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
