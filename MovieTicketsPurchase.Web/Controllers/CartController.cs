using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Domain.Identity;
using MovieTicketsPurchase.Repository;
using MovieTicketsPurchase.Services.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Controllers
{
    [Authorize(Roles = "StandardUser,Admin")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._cartService.getCartInfo(userId));
        }

        public IActionResult DeleteTicketFromCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._cartService.deleteTicketFromCart(userId, id);
            if (result)
            {
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }
        }

        private Boolean OrderNow()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._cartService.orderNow(userId);
            return result;
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = this._cartService.getCartInfo(userId);
            var customer = customerService.Create(new CustomerCreateOptions {
                Email = stripeEmail,
                Source = stripeToken
            });
            var charge = chargeService.Create(new ChargeCreateOptions { 
                Amount = order.TotalPrice * 100,
                Description = "MovieTicketsPurchase Payment",
                Currency = "usd",
                Customer = customer.Id
            });
            if (charge.Status == "succeeded")
            {
                var result = this.OrderNow();
                if (result)
                {
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    return RedirectToAction("Index", "Cart");
                }
            }
            return RedirectToAction("Index", "Cart");
        }
    }
}
