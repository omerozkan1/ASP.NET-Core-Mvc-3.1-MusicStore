using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicStore.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow)
        {
            _logger = logger;
            this.uow = uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = uow.Product.GetAll(includeProperties: "Category,CoverType");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var product = uow.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(cart);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cartModel)
        {
            cartModel.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartModel.AppUserId = claim.Value;

                ShoppingCart fromDb = uow.ShoppingCart.GetFirstOrDefault(s => s.AppUserId == cartModel.AppUserId && s.ProductId == cartModel.ProductId, includeProperties: "Product");

                if (fromDb == null)
                {
                    uow.ShoppingCart.Add(cartModel);
                }
                else
                {
                    fromDb.Count += cartModel.Count;
                }

                uow.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = uow.Product.GetFirstOrDefault(p => p.Id == cartModel.ProductId, includeProperties: "Category,CoverType");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(cart);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
