using BookingService.DTOs;
using BookingService.Models;
using EventDispatcher.Generic;
using Newtonsoft.Json;

namespace BookingService.Extensions
{
    public static class ConverterExtension
    {
        public static BookingDto ToBookingDto(this Booking booking) => new BookingDto
        {
            Id = booking.Id,
            CustomerId = booking.CustomerId,
            FlightId = booking.FlightId,
            PriceWhenBooked = booking.PriceWhenBooked,
            SeatNumber = booking.SeatNumber
        };

        public static Booking ToBooking(
            this BookingDto dto,
            Booking existingBooking = null)
        {

            if (existingBooking == null)
                return new Booking
                {
                    Id = dto.Id,
                    CustomerId = dto.CustomerId,
                    FlightId = dto.FlightId,
                    PriceWhenBooked = dto.PriceWhenBooked,
                    SeatNumber = dto.SeatNumber
                };

            existingBooking.CustomerId = dto.CustomerId;
            existingBooking.FlightId = dto.FlightId;
            existingBooking.PriceWhenBooked = dto.PriceWhenBooked;
            existingBooking.SeatNumber = dto.SeatNumber;

            return existingBooking;
        }

        public static EventMessage<T> ToEventMessage<T>(this Message message) =>
            new EventMessage<T>
            {
                Id = message.Id.ToString(),
                Header = message.Subject,
                Content = JsonConvert.DeserializeObject<T>(message.Content)
            };
    }
}
