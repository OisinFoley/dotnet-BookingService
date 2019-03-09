using BookingService.Data.Abstract;
using Microsoft.AspNetCore.Http;
using BookingService.Models;
using BookingService.ApiResponses;
using BookingService.ApiRequests;
using BookingService.DTOs;
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
    public class BookingController : Controller
    {
        private readonly IBookingRepository m_BookingRepository;
        private readonly IMessageRepository m_MessageRepository;

        public BookingController(IBookingRepository bookingRepository, IMessageRepository messageRepository)
        {
            m_BookingRepository = bookingRepository;
            m_MessageRepository = messageRepository;
        }

        // GET: api/v1/bookings
        [HttpGet("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Booking> bookings = await m_BookingRepository.GetBookings();
            return Ok(bookings);
        }

        // GET: api/v1/bookings/{id}
        [HttpGet("api/v1/bookings/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            var response = new BookingResponse { Booking = booking.ToBookingDto() };
            return Ok(response);
        }

        // POST api/v1/bookings
        [HttpPost("api/v1/bookings")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ObjectResult> Post([FromBody] BookingRequest bookingRequest)
        {
            Booking booking = bookingRequest.Booking.ToBooking();

            booking.Id = Guid.NewGuid();
            var insertedBooking = (Booking)await MessageBusTransactionCall(nameof(Post).ToUpperInvariant(), booking,
                async () => await m_BookingRepository.InsertAsync(booking));

            var response = new BookingResponse { Booking = insertedBooking.ToBookingDto() };
            return Ok(response);
        }

        // PUT: api/v1/bookings/{bookingId}
        [HttpPut("api/v1/bookings/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromBody] BookingRequest bookingRequest)
        {
            if (bookingRequest?.Booking == null)
            {
                return new BadRequestResult();
            }

            bookingRequest.Booking.Id = id;

            if (!(m_BookingRepository.Bookings.Any(x => x.Id == bookingRequest.Booking.Id)))
            {
                return NotFound();
            }

            Booking flight = await GetHydratedBookingAsync(bookingRequest.Booking);
            await m_BookingRepository.UpdateAsync(flight);

            return Ok(flight);
        }

        // DELETE: api/v1/bookings/{bookingId}
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

        private async Task<Booking> GetHydratedBookingAsync(BookingDto dto)
        {
            Booking booking = await m_BookingRepository.FindAsync(dto.Id);
            return dto.ToBooking(booking);
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
