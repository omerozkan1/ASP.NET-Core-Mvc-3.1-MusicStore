using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Models.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
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
            var result = uow.Product.GetAll(includeProperties:"Category");
            return Json(new { data = result });
        } 

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deletedData = uow.Product.Get(id);
            if (deletedData == null)
            {
                return Json(new { success = false, message = "Data Not Found" });
            }

            string rootPath = hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(rootPath, deletedData.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            uow.Product.Remove(deletedData);
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
            ProductViewModel productVm = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = uow.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = uow.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
                return View(productVm);

            productVm.Product = uow.Product.Get(id.GetValueOrDefault());
            if (productVm.Product == null)
                return NotFound();
            return View(productVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productVm)
        {
            if (ModelState.IsValid)
            {
                string rootPath = hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(rootPath, @"uploads\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (productVm.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(rootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVm.Product.ImageUrl = @"\uploads\products\" + fileName + extension;
                }
                else
                {
                    if (productVm.Product.Id != 0)
                    {
                        var productData = uow.Product.Get(productVm.Product.Id);
                        productVm.Product.ImageUrl = productData.ImageUrl;
                    }
                }
                
                if (productVm.Product.Id == 0)
                    uow.Product.Add(productVm.Product);
                else
                    uow.Product.Update(productVm.Product);

                uow.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVm.CategoryList = uow.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                });

                productVm.CoverTypeList = uow.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                if (productVm.Product.Id != 0)
                {
                    productVm.Product = uow.Product.Get(productVm.Product.Id);
                }

            }
            return View(productVm.Product);
        }
    }
}
