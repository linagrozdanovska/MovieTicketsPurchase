using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Domain.Identity;
using MovieTicketsPurchase.Repository;
using MovieTicketsPurchase.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Controllers
{
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

        public IActionResult OrderNow()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._cartService.orderNow(userId);
            if (result)
            {
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}
