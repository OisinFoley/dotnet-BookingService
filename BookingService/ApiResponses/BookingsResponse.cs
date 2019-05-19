using BookingService.DTOs;
using System.Collections.Generic;

namespace BookingService.ApiResponses
{
    public class BookingsResponse
    {
        public IEnumerable<BookingResponseDto> Bookings { get; set; }
    }
}
