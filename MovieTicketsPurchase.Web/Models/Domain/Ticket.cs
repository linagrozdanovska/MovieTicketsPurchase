using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.Domain
{
    public class Ticket
    {
        public Guid TicketId { get; set; }
        public string MovieName { get; set; }
        public DateTime ShowTime { get; set; }
        public int Seat { get; set; }
        public float Price { get; set; }
        public virtual ICollection<TicketInCart> TicketsInCart { get; set; }
    }
}
