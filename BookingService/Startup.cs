using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BookingService.Data;
using BookingService.Data.Abstract;
using BookingService.Data.Concrete;
using Microsoft.Extensions.Hosting;
using BookingService.BackgroundWorkers;
using BookingService.Models;
using EventDispatcher.Azure;
using System.Globalization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace BookingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddDbContext<ApplicationContext>(options => ApplicationContextConfigurator.SetContextOptions(options, Configuration))
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<IUnitOfWork, ApplicationContext>();

            services.AddSingleton<IHostedService, OutboundMessageService<Booking>>(
                serviceProvider =>
                {
                    string topicName = Configuration["OutboundMessageService:TopicName"];
                    string messageBusConnectionString = Configuration["OutboundMessageService:ConnectionString"];
                    
                    var simpleEventSender = new EventSender<Booking>(messageBusConnectionString, topicName);
                    var messageRepository = serviceProvider.GetService<IMessageRepository>();
                    var logger = serviceProvider.GetService<ILogger<OutboundMessageService<Booking>>>();
                    int pollingIntervalInMilliseconds = Convert.ToInt32(Configuration["OutboundMessageService:PollingIntervalInMilliseconds"], CultureInfo.InvariantCulture);

                    return new OutboundMessageService<Booking>(simpleEventSender, messageRepository, logger, pollingIntervalInMilliseconds);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
