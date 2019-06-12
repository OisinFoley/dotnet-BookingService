using BookingService.Data.Abstract;
using Microsoft.AspNetCore.Http;
using BookingService.Models;
using BookingService.ApiResponses;
using BookingService.ApiRequests;
using BookingService.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Transactions;

namespace BookingService.Controllers
{
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IBookingRepository m_BookingRepository;
        private readonly IMessageRepository m_MessageRepository;

        public BookingController(IBookingRepository bookingRepository, IMessageRepository messageRepository)
        {
            m_BookingRepository = bookingRepository;
            m_MessageRepository = messageRepository;
        }

        /// <summary>
        /// Retrieves all known Bookings.
        /// </summary>
        [HttpGet("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookingsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Booking> bookings = await m_BookingRepository.GetBookings();

            var response = new BookingsResponse
            {
                Bookings = bookings.Select(booking => booking.ToBookingResponseDto())
            };
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific Booking.
        /// </summary>
        [HttpGet("api/v1/bookings/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new BadRequestResult();
            }

            Booking booking = await m_BookingRepository.FindAsync(id);

            if (booking == null)
                return NotFound();

            var response = new BookingResponse { Booking = booking.ToBookingResponseDto() };
            return Ok(response);
        }

        /// <summary>
        /// Adds a new Booking.
        /// </summary>
        [HttpPost("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] BookingRequest bookingRequest)
        {
            if (bookingRequest?.Booking == null)
            {
                return new BadRequestResult();
            }

            Booking booking = bookingRequest.Booking.ToBooking();

            booking.Id = Guid.NewGuid();
            var insertedBooking = (Booking)await MessageBusTransactionCall(nameof(Post).ToUpperInvariant(), booking,
                async () => await m_BookingRepository.InsertAsync(booking));

            var response = new BookingResponse { Booking = insertedBooking.ToBookingResponseDto() };
            return Ok(response);
        }

        /// <summary>
        /// Updates a specific existing Booking.
        /// </summary>
        [HttpPut("api/v1/bookings/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromBody] BookingRequest bookingRequest)
        {
            if (bookingRequest?.Booking == null)
            {
                return new BadRequestResult();
            }

            Booking booking = m_BookingRepository.Bookings.SingleOrDefault(x => x.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.CustomerId = bookingRequest.Booking.CustomerId;
            booking.FlightId = bookingRequest.Booking.FlightId;
            booking.PriceWhenBooked = bookingRequest.Booking.PriceWhenBooked;
            booking.SeatNumber = bookingRequest.Booking.SeatNumber;

            var bookingResponse = await m_BookingRepository.UpdateAsync(booking);
            var response = new BookingResponse { Booking = booking.ToBookingResponseDto() };

            return Ok(response);
        }

        /// <summary>
        /// Deletes a specific Booking.
        /// </summary>
        [HttpDelete("api/v1/bookings/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Booking booking = m_BookingRepository.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return new NotFoundResult();
            }

            await m_BookingRepository.DeleteAsync(booking);

            return Ok();
        }

        private async Task<object> MessageBusTransactionCall(string subject, object content, Func<Task<object>> databaseOperation)
        {
            var outboundMessage = new Message
            {
                Subject = subject,
                Content = JsonConvert.SerializeObject(content)
            };

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await m_MessageRepository.InsertAsync(outboundMessage);

                object result = await databaseOperation();

                transaction.Complete();
                return result;
            }
        }
    }
}
