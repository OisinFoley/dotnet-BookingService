using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.DTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        [JsonProperty(PropertyName="customerId")]
        public Guid CustomerId { get; set; }
        [JsonProperty(PropertyName = "flightId")]
        public Guid FlightId { get; set; }
        [JsonProperty(PropertyName = "priceWhenBooked")]
        public int PriceWhenBooked { get; set; }
        [JsonProperty(PropertyName = "seatNumber")]
        public string SeatNumber { get; set; }
    }
}
