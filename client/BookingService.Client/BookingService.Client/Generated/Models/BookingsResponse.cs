// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace BookingService.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class BookingsResponse
    {
        /// <summary>
        /// Initializes a new instance of the BookingsResponse class.
        /// </summary>
        public BookingsResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BookingsResponse class.
        /// </summary>
        public BookingsResponse(IList<BookingResponseDto> bookings = default(IList<BookingResponseDto>))
        {
            Bookings = bookings;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bookings")]
        public IList<BookingResponseDto> Bookings { get; set; }

    }
}
