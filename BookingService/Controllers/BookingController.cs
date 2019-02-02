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
            Booking booking = bookingRequest.Booking.ToBooking();

            booking.Id = Guid.NewGuid();
            var insertedBooking = (Booking)await MessageBusTransactionCall(nameof(Post).ToUpperInvariant(), booking,
                async () => await m_BookingRepository.InsertAsync(booking));

            var response = new BookingResponse { Booking = insertedBooking.ToBookingDto() };
            return response;
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
