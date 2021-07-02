using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Domain.Identity;
using MovieTicketsPurchase.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketsInCart)
                .Include("UserCart.TicketsInCart.Ticket")
                .FirstOrDefaultAsync();
            var userCart = loggedInUser.UserCart;
            var ticketPrice = userCart.TicketsInCart.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            }).ToList();
            var totalPrice = 0;
            foreach (var item in ticketPrice)
            {
                totalPrice += item.TicketPrice * item.Quantity;
            }
            CartDto cartDtoItem = new CartDto
            {
                TicketsInCart = userCart.TicketsInCart.ToList(),
                TotalPrice = totalPrice
            };
            return View(cartDtoItem);
        }

        public async Task<IActionResult> DeleteTicketFromCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketsInCart)
                .Include("UserCart.TicketsInCart.Ticket")
                .FirstOrDefaultAsync();
            var userCart = loggedInUser.UserCart;
            var productToDelete = userCart.TicketsInCart
                .Where(z => z.Id == id)
                .FirstOrDefault();
            userCart.TicketsInCart.Remove(productToDelete);
            _context.Update(userCart);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketsInCart)
                .Include("UserCart.TicketsInCart.Ticket")
                .FirstOrDefaultAsync();
            var userCart = loggedInUser.UserCart;
            Order orderItem = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };
            _context.Add(orderItem);
            List<TicketInOrder> ticketsInOrder = new List<TicketInOrder>();
            ticketsInOrder = userCart.TicketsInCart
                .Select(z => new TicketInOrder
                {
                    OrderId = orderItem.Id,
                    Id = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    UserOrder = orderItem
                }).ToList();
            foreach (var item in ticketsInOrder)
            {
                _context.Add(item);
            }
            loggedInUser.UserCart.TicketsInCart.Clear();
            _context.Update(loggedInUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cart");
        }
    }
}
