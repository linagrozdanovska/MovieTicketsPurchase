using MovieTicketsPurchase.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.Domain
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }
        public virtual ICollection<TicketInCart> TicketsInCart { get; set; }
    }
}
