using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Ecomm_1.DataAccess.Data;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using Project_Ecomm_1.Utility;

namespace Project_Ecomm_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitofWork unitofWork, ApplicationDbContext context)
        {
            _unitofWork = unitofWork;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList=_context.ApplicationUsers.ToList();//Asp.NEtUsers
            var roles=_context.Roles.ToList();              //Asp.netRoles
            var userRoles=_context.UserRoles.ToList(); //Asp.NetUserRoles
            foreach(var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r=>r.Id==roleId).Name;
                if(user.CompanyId !=null)
                {
                    user.Company = new Company()
                    {
                        Name=_unitofWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
                if(user.CompanyId==null)
                {
                    user.Company = new Company()
                    { 
                        Name=""
                    };
                }
            }
            //Remove Admin User
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);

            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnLock([FromBody] string id)
        {
            bool isLocked = false;
            var userInDb=_context.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
            if (userInDb == null)
                return Json(new { success = false, message = "Something went wrong while Lock & Unlock" });
            if(userInDb !=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd= DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked= true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User Successfully Locked" : "User Successfully Unlock" });
        }
        #endregion
    }
}
