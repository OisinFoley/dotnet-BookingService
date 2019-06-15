using System;
using System.Threading.Tasks;
using BookingService.FlightValidation;
using FlightService.Client;
using FlightService.Client.Models;
using Newtonsoft.Json;

namespace BookingService.Extensions
{
    public class FlightValidator: IFlightValidator
    {
        protected readonly IFlightServiceAPI m_FlightServiceClient;

        public FlightValidator(string uri)
        {
            m_FlightServiceClient = new FlightServiceAPI
            {
                BaseUri = new Uri(uri)
            };
        }

        public async Task<bool> HasSeatsAvailable(Guid id)
        {
            var flightGetHttpResponse = await m_FlightServiceClient.Get1WithHttpMessagesAsync(id);
            var flightGetResponseString = await flightGetHttpResponse.Response.Content.ReadAsStringAsync();
            FlightResponse flightGetResponse = JsonConvert.DeserializeObject<FlightResponse>(flightGetResponseString);

            return flightGetResponse.Flight.AvailableSeats > 0;
        }
    }
}
