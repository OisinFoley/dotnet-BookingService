// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace BookingService.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class BookingRequestDto
    {
        /// <summary>
        /// Initializes a new instance of the BookingRequestDto class.
        /// </summary>
        public BookingRequestDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BookingRequestDto class.
        /// </summary>
        public BookingRequestDto(System.Guid? customerId = default(System.Guid?), System.Guid? flightId = default(System.Guid?), int? priceWhenBooked = default(int?), string seatNumber = default(string))
        {
            CustomerId = customerId;
            FlightId = flightId;
            PriceWhenBooked = priceWhenBooked;
            SeatNumber = seatNumber;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerId")]
        public System.Guid? CustomerId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "flightId")]
        public System.Guid? FlightId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priceWhenBooked")]
        public int? PriceWhenBooked { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "seatNumber")]
        public string SeatNumber { get; set; }

    }
}
