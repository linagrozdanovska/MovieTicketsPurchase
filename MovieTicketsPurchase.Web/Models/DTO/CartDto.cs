using MovieTicketsPurchase.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.DTO
{
    public class CartDto
    {
        public List<TicketInCart> TicketsInCart { get; set; }
        public int TotalPrice { get; set; }
    }
}
