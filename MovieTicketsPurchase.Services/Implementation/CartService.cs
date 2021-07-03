using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Repository.Interface;
using MovieTicketsPurchase.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTicketsPurchase.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;

        public CartService(IRepository<Cart> cartRepository, IRepository<TicketInOrder> ticketInOrderRepository, IRepository<Order> orderRepository, IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
        }

        public bool deleteTicketFromCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCart = loggedInUser.UserCart;
                var itemToDelete = userCart.TicketsInCart.Where(z => z.TicketId.Equals(id)).FirstOrDefault();
                userCart.TicketsInCart.Remove(itemToDelete);
                this._cartRepository.Update(userCart);
                return true;
            }
            return false;
        }

        public CartDto getCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);
            var userCart = loggedInUser.UserCart;
            var allTickets = userCart.TicketsInCart.ToList();
            var ticketPrices = allTickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            }).ToList();
            var totalPrice = 0;
            foreach (var item in ticketPrices)
            {
                totalPrice += item.TicketPrice * item.Quantity;
            }
            CartDto cartDto = new CartDto
            {
                TicketsInCart = allTickets,
                TotalPrice = totalPrice
            };
            return cartDto;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCart = loggedInUser.UserCart;
                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser
                };
                this._orderRepository.Insert(order);
                List<TicketInOrder> ticketsInOrder = new List<TicketInOrder>();
                var result = userCart.TicketsInCart.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order
                }).ToList();
                ticketsInOrder.AddRange(result);
                foreach (var item in ticketsInOrder)
                {
                    this._ticketInOrderRepository.Insert(item);
                }
                loggedInUser.UserCart.TicketsInCart.Clear();
                this._userRepository.Update(loggedInUser);
                return true;
            }
            return false;
        }
    }
}
