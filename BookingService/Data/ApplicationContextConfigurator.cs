using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data
{
    public class ApplicationContextConfigurator
    {
        public static void SetContextOptions(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            switch (configuration["DatabaseHost"])
            {
                case "SQLServer":
                    optionsBuilder.UseSqlServer(configuration.GetConnectionString("SQLServer"));
                    break;
                default:
                    throw new ArgumentException("No Database Host provided. - Specify a valid connection string in appsettings.json");
            }
        }
    }
}
