using Microsoft.AspNetCore.Identity;
using MovieTicketsPurchase.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Cart UserCart { get; set; }
    }
}
