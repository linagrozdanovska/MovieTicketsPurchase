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
        private readonly IRepository<EmailMessage> _mailRepository;

        public CartService(IRepository<Cart> cartRepository, IRepository<TicketInOrder> ticketInOrderRepository, IRepository<Order> orderRepository, IUserRepository userRepository, IRepository<EmailMessage> mailRepository)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _mailRepository = mailRepository;
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
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.MailTo = loggedInUser.Email;
                emailMessage.Subject = "Successfully created order";
                emailMessage.Status = false;
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
                    UserOrder = order,
                    Quantity = z.Quantity
                }).ToList();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Your order is completed. The order contains the following tickets: ");
                int totalQuantity = 0;
                int totalPrice = 0;
                for (int i = 1; i <= result.Count; i++)
                {
                    var item = result[i-1];
                    totalQuantity += item.Quantity;
                    totalPrice += item.Quantity * item.SelectedTicket.Price;
                    stringBuilder.AppendLine(i.ToString() + ". Movie: " + item.SelectedTicket.MovieName + ", Quantity: " + item.Quantity + ", Price: " + item.SelectedTicket.Price + " MKD");
                }
                stringBuilder.AppendLine("Total Quantity: " + totalQuantity.ToString());
                stringBuilder.AppendLine("Total Price: " + totalPrice.ToString() + " MKD");
                emailMessage.Content = stringBuilder.ToString();
                ticketsInOrder.AddRange(result);
                foreach (var item in ticketsInOrder)
                {
                    this._ticketInOrderRepository.Insert(item);
                }
                loggedInUser.UserCart.TicketsInCart.Clear();
                this._mailRepository.Insert(emailMessage);
                this._userRepository.Update(loggedInUser);
                return true;
            }
            return false;
        }
    }
}
