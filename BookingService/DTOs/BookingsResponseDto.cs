using System.Collections.Generic;
namespace BookingService.DTOs
{
    public class BookingsResponseDto
    {
        public IEnumerable<BookingResponseDto> Bookings { get; set; }
    }
}
