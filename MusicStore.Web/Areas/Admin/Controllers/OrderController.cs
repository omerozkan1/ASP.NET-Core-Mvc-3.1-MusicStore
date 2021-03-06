using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Order> orderList;

            if (User.IsInRole(ProjectConstant.Role_Admin) || User.IsInRole(ProjectConstant.Role_Employee))
                orderList = uow.Order.GetAll(includeProperties: "AppUser");
            else
                orderList = uow.Order.GetAll(u => u.AppUserId == claim.Value, includeProperties: "AppUser");

            switch (status)
            {
                case "pending":
                    orderList = orderList.Where(o => o.PaymentStatus == ProjectConstant.PaymentStatusDelayed);
                    break;

                case "inprocess":
                    orderList = orderList.Where(o => o.PaymentStatus == ProjectConstant.StatusApproved || o.OrderStatus == ProjectConstant.StatusInProcess || o.OrderStatus == ProjectConstant.StatusPending);
                    break;

                case "completed":
                    orderList = orderList.Where(o => o.PaymentStatus == ProjectConstant.StatusShipped);
                    break;

                case "rejected":
                    orderList = orderList.Where(o => o.PaymentStatus == ProjectConstant.StatusCanceled || o.OrderStatus == ProjectConstant.StatusRefund || o.OrderStatus == ProjectConstant.PaymentStatusRejected);
                    break;

                default:
                    break;
            }

            //orderList = uow.Order.GetAll(includeProperties: "AppUser");
            return Json(new { data = orderList });
        }
    }
}
