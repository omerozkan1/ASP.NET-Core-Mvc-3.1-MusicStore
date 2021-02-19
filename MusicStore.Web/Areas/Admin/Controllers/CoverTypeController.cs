using Dapper;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        #region Variables
        private readonly IUnitOfWork uow; 
        #endregion

        #region Ctor
        public CoverTypeController(IUnitOfWork uow)
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
            var allCoverTypes = uow.sp_call.List<CoverType>(ProjectConstant.Proc_CoverType_GetAll,null);
            return Json(new { data = allCoverTypes });
        } 

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            var deletedData = uow.sp_call.OneRecord<CoverType>(ProjectConstant.Proc_CoverType_Get, parameter);
            if (deletedData == null)
                return Json(new { success = false, message = "Data Not Found" });

            uow.sp_call.Execute(ProjectConstant.Proc_CoverType_Delete,parameter);
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
            CoverType coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
            }

            var parameter = new DynamicParameters();
            parameter.Add("@Id",id);

            coverType = uow.sp_call.OneRecord<CoverType>(ProjectConstant.Proc_CoverType_Get,parameter);
            if (coverType != null)
                return View(coverType);

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                    uow.sp_call.Execute(ProjectConstant.Proc_CoverType_Create, parameter);
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    uow.sp_call.Execute(ProjectConstant.Proc_CoverType_Update, parameter);
                }
                    
                uow.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }
    }
}
