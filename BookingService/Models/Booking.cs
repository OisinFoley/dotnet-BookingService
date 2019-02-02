using System;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid FlightId { get; set; }
        public int PriceWhenBooked { get; set; }
        public string SeatNumber { get; set; }
    }
}
