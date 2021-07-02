using MovieTicketsPurchase.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Domain.DomainModels
{
    public class Cart : BaseEntity
    {
        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }
        public virtual ICollection<TicketInCart> TicketsInCart { get; set; }
    }
}
