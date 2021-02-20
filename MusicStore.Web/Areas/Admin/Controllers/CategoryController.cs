using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;
using System.Linq;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        #region Api Calls
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
 
    }
}
