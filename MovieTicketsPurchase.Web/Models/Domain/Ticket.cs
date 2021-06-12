﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.Domain
{
    public class Ticket
    {
        public Guid TicketId { get; set; }
        [Display(Name = "Poster")]
        public string MovieImage { get; set; }
        [Required]
        [Display(Name = "Movie")]
        public string MovieName { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string MovieDescription { get; set; }
        [Required]
        [Display(Name = "Genre")]
        public string MovieGenre { get; set; }
        [Required]
        [Display(Name = "Showtime")]
        public DateTime ShowTime { get; set; }
        [Required]
        [Display(Name = "Ticket Price (MKD)")]
        public int Price { get; set; }
        public virtual ICollection<TicketInCart> TicketsInCart { get; set; }
    }
}
