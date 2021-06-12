using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Models.Domain
{
    public class Movie
    {
        public Guid MovieId { get; set; }
        public string MovieImage { get; set; }
        public string MovieName { get; set; }
        public string MovieDescription { get; set; }
        public string MovieGenre { get; set; }
        public float MovieRating { get; set; }
        public DateTime Showtime { get; set; }
        public int[] Seats = new int[200];
    }
}
