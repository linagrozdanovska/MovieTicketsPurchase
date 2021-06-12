﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Web.Models.Domain;
using MovieTicketsPurchase.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketsPurchase.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<TicketInCart> TicketsInCart { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Movie>()
                .Property(z => z.MovieId)
                .ValueGeneratedOnAdd();

            builder.Entity<Cart>()
               .Property(z => z.CartId)
               .ValueGeneratedOnAdd();

            builder.Entity<Ticket>()
               .Property(z => z.TicketId)
               .ValueGeneratedOnAdd();

            builder.Entity<TicketInCart>()
                .HasKey(z => new { z.TicketId, z.CartId });

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
        }
    }
}
