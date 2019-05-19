using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BookingService.Models;
using Xunit;
using BookingService;
using BookingService.ApiRequests;
using BookingService.DTOs;
using BookingService.ApiResponses;
//using BookingService.ApiRequests;

namespace BookingService.IntegrationTests
{
    public class BookingTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;
        private const string GetBookingGuid = "a986b31d-cd01-4657-9c6c-1dc3b45f437c";
        private const string DeleteBookingGuid = "4e8eeed0-7a55-4588-aef4-de425ad0d8ab";


        private static BookingRequestDto reusableBookingDto = new BookingRequestDto
        {
            CustomerId = new Guid("22265242-322e-4a15-bb4a-07fa7eb992a7"),
            FlightId = new Guid("f39a67ca-9d4d-45c7-ab0d-559e7173477b"),
            PriceWhenBooked = 1000,
            SeatNumber = "25ZZ"
        };
        private BookingRequest reusableBookingRequest = 
            new BookingRequest { Booking = reusableBookingDto };

        public BookingTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task TestGetBookingsAsync()
        {
            // Arrange
            var request = "/api/v1/bookings";

            // Act
            var response = await Client.GetAsync(request);
            var contents = await response.Content.ReadAsStringAsync();


            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetSingleBookingAsync()
        {
            // Arrange
            var request = $"/api/v1/bookings/{GetBookingGuid}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPostSingleBookingAsync()
        {

            // WE WANT TO REMOVE THE Id LATER AND ACTUALLY GRAB THE RETURNED GUID 
            // BEFORE UPDATING VIA PUT REQUEST
            // Arrange
            var request = new
            {
                Url = "/api/v1/bookings",
                Body = reusableBookingRequest
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPutSingleBookingAsync()
        {
            // Arrange
            var request = new
            {
                Url = $"/api/v1/bookings/{GetBookingGuid}",
                Body = reusableBookingRequest
            };

            // Act
            var response = await Client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestDeleteStockItemAsync()
        {
            // Arrange
            var postRequest = new
            {
                Url = "/api/v1/bookings",
                Body = reusableBookingRequest
            };

            // Act
            var postResponse = await Client.PostAsync(postRequest.Url, ContentHelper.GetStringContent(postRequest.Body));
            var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<BookingResponse>(jsonFromPostResponse);
            var deleteRequest = $"/api/v1/bookings/{singleResponse.Booking.Id}";
            var deleteResponse = await Client.DeleteAsync(deleteRequest);

            // Assert
            postResponse.EnsureSuccessStatusCode();
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}