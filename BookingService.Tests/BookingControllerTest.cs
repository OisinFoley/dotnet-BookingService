using BookingService.ApiResponses;
using BookingService.Controllers;
using BookingService.Data.Abstract;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingService.Tests
{
    public class BookingControllerTest
    {
        private readonly IBookingRepository m_BookingRepository;
        private readonly IMessageRepository m_MessageRepository;
        private readonly List<Booking> m_Bookings;

        private const string Guid1 = "eb1e1206-d482-4b83-a613-a6db91ce732a";
        private const string CustomerId1 = "448bd1df-6376-4c8a-a9aa-b0d673bea3b6";
        private const string FlightId1 = "05651d29-6354-468d-82ac-2b6bf613a753";
        private const int PriceWhenBooked1 = 100;
        private const string SeatNumber1 = "15B";

        private const string Guid2 = "ab3147bb-25d3-4c6b-bbdd-42d658b205c3";
        private const string CustomerId2 = "ad80c8e8-f708-4b7f-a29a-1f4eef4aad30";
        private const string FlightId2 = "3d1275cb-b540-4799-aa3d-2458b61d152d";
        private const int PriceWhenBooked2 = 200;
        private const string SeatNumber2 = "30B";

        private const string Guid3 = "b431a2ab-730c-4df0-8e9d-81fc74b5b4b8";
        private Guid CustomerId3 = new Guid("26797c2c-cfea-4276-b67d-56973dfe634b");
        private Guid FlightId3 = new Guid("2fec60bc-7244-41b1-8e45-c8b6a2dbb1cc");
        private const int PriceWhenBooked3 = 300;
        private const string SeatNumber3  = "45C";

        public BookingControllerTest()
        {
            m_Bookings = new List<Booking>
            {
                new Booking { Id = new Guid(Guid1), CustomerId = new Guid(CustomerId1), FlightId = new Guid(FlightId1), PriceWhenBooked = PriceWhenBooked1, SeatNumber = SeatNumber1},
                new Booking { Id = new Guid(Guid2), CustomerId = new Guid(CustomerId2), FlightId = new Guid(FlightId2), PriceWhenBooked = PriceWhenBooked2, SeatNumber = SeatNumber2},
            };

            m_BookingRepository = Substitute.For<IBookingRepository>();
            m_MessageRepository = Substitute.For<IMessageRepository>();

            m_BookingRepository.Bookings.ReturnsForAnyArgs(m_Bookings);
            m_BookingRepository.GetBookings()
                .Returns(m_Bookings);

            m_BookingRepository.FindAsync(Arg.Any<object[]>())
                .Returns(x => m_BookingRepository.Bookings.SingleOrDefault(booking => booking.Id == (Guid)x.Arg<object[]>()[0]));

            m_BookingRepository.InsertAsync(Arg.Any<Booking>())
                .Returns(x => x.Arg<Booking>())
                .AndDoes(y => m_Bookings.Add(y.Arg<Booking>()));
            m_BookingRepository.UpdateAsync(Arg.Any<Booking>())
                .Returns(0)
                .AndDoes(y =>
                {
                    m_Bookings.RemoveAll(w => w.Id == y.Arg<Booking>().Id);
                    m_Bookings.Add(y.Arg<Booking>());
                });
            m_BookingRepository.DeleteAsync(Arg.Any<Booking>())
                .Returns(0)
                .AndDoes(y => m_Bookings.Remove(y.Arg<Booking>()));
        }

        #region GetBooking(s)

        [Fact]
        public async Task GetAllBookingsReturnsOkObjectResponseWhenRecordsFound()
        {
            // Arrange
            var bookingController = new BookingController(m_BookingRepository, m_MessageRepository);

            // Act
            IActionResult result = await bookingController.Get().ConfigureAwait(false);
            var parsedResult = result as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotEmpty((IEnumerable<Booking>)parsedResult.Value);
            Assert.True(parsedResult.StatusCode == 200);
        }

        [Fact]
        public async Task GetBookingByIdReturnsBadRequestWhenGuidIsNull()
        {
            // Arrange
            var bookingController = new BookingController(m_BookingRepository, m_MessageRepository);

            // Act
            IActionResult result = await bookingController.Get(Guid.Empty).ConfigureAwait(false);
            var parsedResult = result as BadRequestResult;

            // Assert
            Assert.IsType<BadRequestResult>(result);
            Assert.True(parsedResult.StatusCode == 400);
        }

        [Fact]
        public async Task GetByGuidReturnsNotFoundResultWhenNoResults()
        {
            // Arrange
            var bookingController = new BookingController(m_BookingRepository, m_MessageRepository);

            // Act
            IActionResult result = await bookingController.Get(new Guid(Guid3)).ConfigureAwait(false);
            var parsedResult = result as NotFoundResult;

            // Assert
            Assert.IsType<NotFoundResult>(result);
            Assert.True(parsedResult.StatusCode == 404);
        }

        
        [Fact]
        public async Task GetBookingByIdReturnsTheCorrectObjectWhenFound()
        {
            // Arrange
            m_Bookings.Add(new Booking { Id = new Guid(Guid3), CustomerId = CustomerId3, FlightId = FlightId3, PriceWhenBooked = PriceWhenBooked3, SeatNumber = SeatNumber3 });
            var bookingController = new BookingController(m_BookingRepository, m_MessageRepository);

            // Act
            IActionResult result = await bookingController.Get(new Guid(Guid2)).ConfigureAwait(false);
            var parsedResult = result as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.True(parsedResult.StatusCode == 200);

            var response = Assert.IsType<BookingResponse>((parsedResult.Value));

            BookingDto booking2 = response.Booking;
            Assert.Equal(CustomerId2, booking2.CustomerId.ToString());
            Assert.Equal(FlightId2, booking2.FlightId.ToString());
            Assert.Equal(PriceWhenBooked2, booking2.PriceWhenBooked);
            Assert.Equal(SeatNumber2, booking2.SeatNumber);
        }
        
        #endregion
    }
}
