using BookingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data.Abstract
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        IEnumerable<Booking> Bookings { get; }

        Task<List<Booking>> GetBookings();
    }
}
