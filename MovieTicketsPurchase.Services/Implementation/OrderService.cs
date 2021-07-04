using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Repository.Interface;
using MovieTicketsPurchase.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public List<Order> GetAllOrders(string userId)
        {
            return this._orderRepository.GetAllOrders(userId);
        }

        public Order GetOrderDetails(string userId, Guid id)
        {
            return this._orderRepository.GetOrderDetails(userId, id);
        }
    }
}
