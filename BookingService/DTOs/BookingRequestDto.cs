using System;
namespace BookingService.DTOs
{
    public class BookingRequestDto
    {
        public Guid CustomerId { get; set; }

        public Guid FlightId { get; set; }

        public int PriceWhenBooked { get; set; }

        public string SeatNumber { get; set; }
    }
}
