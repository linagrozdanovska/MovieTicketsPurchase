using MovieTicketsPurchase.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Services.Interface
{
    public interface IOrderService
    {
        List<Order> GetAllOrders(string userId);
    }
}
