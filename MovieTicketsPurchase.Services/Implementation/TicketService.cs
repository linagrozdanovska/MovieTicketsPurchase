using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Repository.Interface;
using MovieTicketsPurchase.Services.Interface;

namespace MovieTicketsPurchase.Services.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInCart> _ticketInCartRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(IRepository<Ticket> ticketRepository, IUserRepository userRepository, IRepository<TicketInCart> ticketInCartRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketInCartRepository = ticketInCartRepository;
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.GetAll().ToList();
        }

        public Ticket GetDetailsForTicket(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }
        
        public void CreateNewTicket(Ticket t)
        {
            this._ticketRepository.Insert(t);
        }

        public void UpdeteExistingTicket(Ticket t)
        {
            this._ticketRepository.Update(t);
        }

        public AddToCartDto GetCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            AddToCartDto model = new AddToCartDto
            {
                SelectedTicket = ticket,
                Id = ticket.Id,
                Quantity = 1
            };
            return model;
        }

        public void DeleteTicket(Guid id)
        {
            var ticket = this.GetDetailsForTicket(id);
            this._ticketRepository.Delete(ticket);
        }

        public bool AddToCart(AddToCartDto item, string userID)
        {
            var user = this._userRepository.Get(userID);
            var userCart = user.UserCart;
            if (item.Id != null && userCart != null)
            {
                var ticket = this.GetDetailsForTicket(item.Id);
                if (ticket != null)
                {
                    TicketInCart itemToAdd = new TicketInCart
                    {
                        Ticket = ticket,
                        Id = ticket.Id,
                        Cart = userCart,
                        CartId = userCart.Id,
                        Quantity = item.Quantity
                    };
                    this._ticketInCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
