using BookingService.Data.Abstract;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data
{
    public class ApplicationContext : DbContext, IUnitOfWork
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }

        async Task<int> IUnitOfWork.SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
