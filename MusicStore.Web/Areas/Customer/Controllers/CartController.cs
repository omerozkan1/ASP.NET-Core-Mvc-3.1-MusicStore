using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MusicStore.Core.Const;
using MusicStore.Core.Helper;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Models.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MusicStore.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IEmailSender emailSender;
        private readonly UserManager<IdentityUser> userManager;
        public CartController(IUnitOfWork uow, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            this.uow = uow;
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                Order = new Models.DbModels.Order(),
                CartList = uow.ShoppingCart.GetAll(u => u.AppUserId == claims.Value, includeProperties: "Product")
            };

            ShoppingCartViewModel.Order.OrderTotal = 0;
            ShoppingCartViewModel.Order.AppUser = uow.AppUser.GetFirstOrDefault(u => u.Id == claims.Value, includeProperties: "Company");

            foreach (var cart in ShoppingCartViewModel.CartList)
            {
                cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartViewModel.Order.OrderTotal += (cart.Price * cart.Count);
                cart.Product.Description = ProjectConstant.ConvertToRawHtml(cart.Product.Description);
                if (cart.Product.Description.Length > 50)
                {
                    cart.Product.Description = cart.Product.Description.Substring(0, 49) + "....";
                }
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = uow.AppUser.GetFirstOrDefault(u => u.Id == claims.Value);

            if (user == null)
                ModelState.AddModelError(string.Empty, "Verification email is empty!");

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            ModelState.AddModelError(string.Empty, "Verification email sent.Please check your email!");

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Plus(int cartId)
        {
            var cart = uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");

            if (cart == null)
                return RedirectToAction(nameof(Index));

            cart.Count += 1;
            cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

            uow.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");

            if (cart.Count == 1)
            {
                var cnt = uow.ShoppingCart.GetAll(u => u.AppUserId == cart.AppUserId).ToList().Count();
                uow.ShoppingCart.Remove(cart);
                uow.Save();
                HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                uow.Save();
            }
            return RedirectToAction(nameof(Index));
                
        }

        public IActionResult Remove(int cartId)
        {
            var cart = uow.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            var cnt = uow.ShoppingCart.GetAll(u => u.AppUserId == cart.AppUserId).ToList().Count;
            uow.ShoppingCart.Remove(cart);
            uow.Save();
            HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, cnt - 1);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                Order = new Models.DbModels.Order(),
                CartList = uow.ShoppingCart.GetAll(u => u.AppUserId == claims.Value, includeProperties: "Product")
            };

            ShoppingCartViewModel.Order.AppUser = uow.AppUser.GetFirstOrDefault(u => u.Id == claims.Value, includeProperties: "Company");

            foreach (var item in ShoppingCartViewModel.CartList)
            {
                item.Price = ProjectConstant.GetPriceBaseOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartViewModel.Order.OrderTotal += (item.Price * item.Count);
            }

            ShoppingCartViewModel.Order.Name = ShoppingCartViewModel.Order.AppUser.Name;
            ShoppingCartViewModel.Order.PhoneNumber = ShoppingCartViewModel.Order.AppUser.PhoneNumber;
            ShoppingCartViewModel.Order.StreetAddress = ShoppingCartViewModel.Order.AppUser.StreetAddress;
            ShoppingCartViewModel.Order.City = ShoppingCartViewModel.Order.AppUser.City;
            ShoppingCartViewModel.Order.State = ShoppingCartViewModel.Order.AppUser.State;
            ShoppingCartViewModel.Order.PostalCode = ShoppingCartViewModel.Order.AppUser.PostalCode;

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel.Order.AppUser = uow.AppUser.GetFirstOrDefault(a => a.Id == claims.Value, includeProperties: "Company");
            ShoppingCartViewModel.CartList = uow.ShoppingCart.GetAll(s => s.AppUserId == claims.Value, includeProperties: "Product");

            ShoppingCartViewModel.Order.PaymentStatus = ProjectConstant.PaymentStatusPending;
            ShoppingCartViewModel.Order.OrderStatus = ProjectConstant.PaymentStatusPending;
            ShoppingCartViewModel.Order.AppUserId = claims.Value;
            ShoppingCartViewModel.Order.OrderDate = DateTime.Now;

            uow.Order.Add(ShoppingCartViewModel.Order);
            uow.Save();

            List<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (var orderDetail in ShoppingCartViewModel.CartList)
            {
                orderDetail.Price = ProjectConstant.GetPriceBaseOnQuantity(orderDetail.Count, orderDetail.Product.Price, orderDetail.Product.Price50, orderDetail.Product.Price100);

                OrderDetail detail = new OrderDetail()
                {
                    ProductId = orderDetail.ProductId,
                    OrderId = ShoppingCartViewModel.Order.Id,
                    Price = orderDetail.Price,
                    Count = orderDetail.Count
                };

                ShoppingCartViewModel.Order.OrderTotal += detail.Count * detail.Price;
                uow.OrderDetail.Add(detail);
            }
            uow.ShoppingCart.RemoveRange(ShoppingCartViewModel.CartList);
     
            HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, 0);

            if (stripeToken == null)
            {

            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartViewModel.Order.OrderTotal * 100),
                    Currency = "try",
                    Description = "Order Id : " + ShoppingCartViewModel.Order.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                    ShoppingCartViewModel.Order.PaymentStatus = ProjectConstant.PaymentStatusRejected;
                else
                    ShoppingCartViewModel.Order.TransactionId = charge.BalanceTransactionId;

                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartViewModel.Order.PaymentStatus = ProjectConstant.PaymentStatusApproved;
                    ShoppingCartViewModel.Order.OrderStatus = ProjectConstant.StatusApproved;
                    ShoppingCartViewModel.Order.PaymentDate = DateTime.Now;
                }

            }

            uow.Save();
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartViewModel.Order.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
