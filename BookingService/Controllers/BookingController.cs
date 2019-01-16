using BookingService.Data.Abstract;
using Microsoft.AspNetCore.Http;
using BookingService.Models;
using BookingService.ApiResponses;
using BookingService.ApiRequests;
using BookingService.DTOs;
using BookingService.Extensions;

namespace BookingService.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository m_BookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            m_BookingRepository = bookingRepository;
        }

        // GET api/v1/flights
        [HttpGet("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<OkObjectResult> Get(Guid flightId)
        {
            IEnumerable<Booking> bookings = await m_BookingRepository.GetBookings();

            bookings = bookings.Where(b => b.FlightId == flightId);

            return new OkObjectResult(bookings);
        }

        // POST api/v1/flights
        [HttpPost("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<BookingResponse> Post([FromBody] BookingRequest bookingRequest)
        {
            Booking insertedBooking = await m_BookingRepository.InsertAsync(await GetHydratedBookingAsync(bookingRequest.Booking));

            var response = new BookingResponse { Booking = insertedBooking.ToBookingDto() };
            return response;
        }

        private async Task<Booking> GetHydratedBookingAsync(BookingDto dto)
        {
            Booking booking = await m_BookingRepository.FindAsync(dto.Id);
            return dto.ToBooking(booking);
        }
    }
}
