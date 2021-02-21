using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
    public class CompanyController : Controller
    {
        #region Variables
        private readonly IUnitOfWork uow; 
        #endregion

        #region Ctor
        public CompanyController(IUnitOfWork uow)
        {
            this.uow = uow;
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
            var result = uow.Company.GetAll();
            return Json(new { data = result });
        } 

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deletedData = uow.Company.Get(id);
            if (deletedData == null)
            {
                return Json(new { success = false, message = "Data Not Found" });
            }
            uow.Company.Remove(deletedData);
            uow.Save();
            return Json(new { success = true, message = "Delete Operation Successfully" });
        }
        #endregion

        /// <summary>
        /// Create or update get method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                return View(company);
            }
            company = uow.Company.Get((int)id);
            if (company != null)
                return View(company);

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                    uow.Company.Add(company);
                else
                    uow.Company.Update(company);

                uow.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
    }
}
