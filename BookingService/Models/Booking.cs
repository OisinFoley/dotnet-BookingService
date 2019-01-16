using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid FlightId { get; set; }
        public int PriceWhenBooked { get; set; }
        public string SeatNumber { get; set; }
    }
}
