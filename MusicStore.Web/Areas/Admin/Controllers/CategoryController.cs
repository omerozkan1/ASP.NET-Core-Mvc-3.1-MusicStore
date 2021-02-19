using Microsoft.AspNetCore.Mvc;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        #region Variables
        private readonly IUnitOfWork uow; 
        #endregion

        #region Ctor
        public CategoryController(IUnitOfWork uow)
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
            var result = uow.category.GetAll();
            return Json(new { data = result });
        } 

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deletedDate = uow.category.Get(id);
            if (deletedDate == null)
            {
                return Json(new { success = false, message = "Data Not Found" });
            }
            uow.category.Remove(deletedDate);
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
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = uow.category.Get((int)id);
            if (category != null)
                return View(category);

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                    uow.category.Add(category);
                else
                    uow.category.Update(category);

                uow.Save();
                return RedirectToAction(nameof(Index));
            }
                return View(category);
            }
            return NotFound();
        }
    }
}
