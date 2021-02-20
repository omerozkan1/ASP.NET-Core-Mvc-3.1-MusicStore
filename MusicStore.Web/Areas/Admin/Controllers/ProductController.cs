using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Models.ViewModels;
using System.Linq;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        #region Variables
        private readonly IUnitOfWork uow;
        private readonly IWebHostEnvironment hostEnvironment;
        #endregion

        #region Ctor
        public ProductController(IUnitOfWork uow, IWebHostEnvironment hostEnvironment)
        {
            this.uow = uow;
            this.hostEnvironment = hostEnvironment;
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
            var result = uow.product.GetAll();
            return Json(new { data = result });
        } 

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deletedDate = uow.product.Get(id);
            if (deletedDate == null)
            {
                return Json(new { success = false, message = "Data Not Found" });
            }
            uow.product.Remove(deletedDate);
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
            ProductViewModel product = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = uow.category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = uow.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
                return View(product);

            product.Product = uow.product.Get(id.GetValueOrDefault());
            if (product.Product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Id == 0)
                    uow.product.Add(product);
                else
                    uow.product.Update(product);

                uow.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
