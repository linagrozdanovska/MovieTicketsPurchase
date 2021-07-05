using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Repository
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<TicketInCart> TicketsInCart { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<TicketInOrder> TicketsInOrder { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Cart>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<Ticket>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();

            //builder.Entity<TicketInCart>()
            //    .HasKey(z => new { z.Id, z.CartId });

            builder.Entity<TicketInCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketsInCart)
                .HasForeignKey(z => z.CartId);

            builder.Entity<TicketInCart>()
                .HasOne(z => z.Cart)
                .WithMany(z => z.TicketsInCart)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<Cart>()
                .HasOne<AppUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<Cart>(z => z.OwnerId);

            //builder.Entity<TicketInOrder>()
            //    .HasKey(z => new { z.TicketId, z.OrderId });

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.SelectedTicket)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.OrderId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.TicketId);
        }
    }
}
