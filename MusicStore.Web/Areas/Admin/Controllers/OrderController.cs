using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using System.Collections.Generic;

namespace MusicStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork uow;
        public OrderController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetOrderList()
        {
            IEnumerable<Order> orderList;
            orderList = uow.Order.GetAll(includeProperties: "AppUser");
            return Json(new { data = orderList });
        }
    }
}
