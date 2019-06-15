using System;
using System.Threading.Tasks;

namespace BookingService.FlightValidation
{
    public interface IFlightValidator
    {
        Task<bool> HasSeatsAvailable(Guid id);
    }
}
