using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTicketsPurchase.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }

        public List<Order> GetAllOrders(string userId)
        {
            return entities
                .Where(z => z.UserId.Equals(userId))
                .Include(z => z.TicketsInOrder)
                .Include("TicketsInOrder.SelectedTicket")
                .Include(z => z.User)
                .ToListAsync().Result;
        }

        public Order GetOrderDetails(string userId, Guid id)
        {
            return entities
                .Where(z => z.UserId.Equals(userId))
                .Include(z => z.TicketsInOrder)
                .Include("TicketsInOrder.SelectedTicket")
                .Include(z => z.User)
                .SingleOrDefaultAsync(z => z.Id == id).Result;
        }
    }
}
