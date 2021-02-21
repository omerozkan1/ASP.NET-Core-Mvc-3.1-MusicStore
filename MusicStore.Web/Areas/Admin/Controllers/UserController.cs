using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;
using System;
using System.Linq;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class UserController : Controller
    {
        #region Variables
        private readonly ApplicationDbContext db;
        #endregion

        #region Ctor
        public UserController(ApplicationDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        public IActionResult GetAll()
        {
            var userList = db.AppUsers.Include(c => c.Company).ToList();
            var userRole = db.UserRoles.ToList();
            var roles = db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = string.Empty
                    };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var data = db.AppUsers.FirstOrDefault(u => u.Id == id);
            if (data == null)
                return Json(new { success = false, message = "Error while locking/unlocking" });

            if (data.LockoutEnd != null && data.LockoutEnd > DateTime.Now)
                data.LockoutEnd = DateTime.Now;

            else
                data.LockoutEnd = DateTime.Now.AddYears(10);

            db.SaveChanges();
            return Json(new { success = true, message = "Operation succesfully" });
        }

    }
}
