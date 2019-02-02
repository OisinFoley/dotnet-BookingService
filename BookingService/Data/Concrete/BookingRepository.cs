using BookingService.Data.Abstract;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data.Concrete
{
    public sealed class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        IEnumerable<Booking> IBookingRepository.Bookings => GetDbSet().AsEnumerable();

        public async Task<List<Booking>> GetBookings() =>
            await GetDbSet().ToListAsync();
    }
}
