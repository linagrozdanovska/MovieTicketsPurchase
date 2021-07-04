using MovieTicketsPurchase.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders(string UserId);
        Order GetOrderDetails(string userId, Guid id);
    }
}
