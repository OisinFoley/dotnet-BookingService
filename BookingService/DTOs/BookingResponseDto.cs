using System;
namespace BookingService.DTOs
{
    public class BookingResponseDto: BookingRequestDto
    {
        public Guid Id { get; set; }
    }
}
