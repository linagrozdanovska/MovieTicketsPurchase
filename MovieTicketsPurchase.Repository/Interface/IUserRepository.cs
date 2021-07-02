using MovieTicketsPurchase.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<AppUser> GetAll();
        AppUser Get(string id);
        void Insert(AppUser entity);
        void Update(AppUser entity);
        void Delete(AppUser entity);
    }
}
