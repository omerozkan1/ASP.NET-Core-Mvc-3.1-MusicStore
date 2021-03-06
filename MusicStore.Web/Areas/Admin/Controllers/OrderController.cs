using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Core.Const;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Models.ViewModels;
using Stripe;
using System;
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

        [BindProperty]
        public OrderDetailViewModel OrderDetailViewModel { get; set; }
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
            IEnumerable<Models.DbModels.Order> orderList;

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
                    orderList = orderList.Where(o => o.PaymentStatus == ProjectConstant.StatusCancelled || o.OrderStatus == ProjectConstant.StatusRefund || o.OrderStatus == ProjectConstant.PaymentStatusRejected);
                    break;

                default:
                    break;
            }

            //orderList = uow.Order.GetAll(includeProperties: "AppUser");
            return Json(new { data = orderList });
        }

        public IActionResult Details(int id)
        {
            OrderDetailViewModel = new OrderDetailViewModel
            {
                Order = uow.Order.GetFirstOrDefault(u => u.Id == id, includeProperties: "AppUser"),
                OrderDetails = uow.OrderDetail.GetAll(o => o.OrderId == id, includeProperties: "Product")
            };

            return View(OrderDetailViewModel);
        }

        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult StartProcessing(int id) 
        {
            Models.DbModels.Order order = uow.Order.GetFirstOrDefault(u => u.Id == id);
            order.OrderStatus = ProjectConstant.StatusInProcess;
            uow.Save();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult ShipOrder()
        {
            Models.DbModels.Order order = uow.Order.GetFirstOrDefault(u => u.Id == OrderDetailViewModel.Order.Id);
            order.TrackNumber = OrderDetailViewModel.Order.TrackNumber;
            order.Carrier = OrderDetailViewModel.Order.Carrier;
            order.OrderStatus = ProjectConstant.StatusShipped;
            order.ShippingDate = DateTime.Now;

            uow.Save();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            Models.DbModels.Order order = uow.Order.GetFirstOrDefault(u => u.Id == id);
            if (order.PaymentStatus == ProjectConstant.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(order.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = order.TransactionId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                order.OrderStatus = ProjectConstant.StatusRefund;
                order.PaymentStatus = ProjectConstant.StatusRefund;
            }
            else
            {
                order.OrderStatus = ProjectConstant.StatusCancelled;
                order.PaymentStatus = ProjectConstant.StatusCancelled;
            }

            uow.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
