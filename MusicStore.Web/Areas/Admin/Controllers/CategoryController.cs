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
            {
                return View(category);
            }
            return NotFound();
        }
    }
}
