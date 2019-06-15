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
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using FlightService.Client;
using BookingService.FlightValidation;
using BookingService.Extensions;

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

            var flightValidator = new FlightValidator(Configuration["FlightServiceClient:BaseUri"]);

            services
                .AddDbContext<ApplicationContext>(options => ApplicationContextConfigurator.SetContextOptions(options, Configuration))
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<IFlightServiceAPI, FlightServiceAPI>()
                .AddScoped<IUnitOfWork, ApplicationContext>();
            
            services.AddScoped<IFlightValidator, FlightValidator>(
                serviceProvider =>
                {
                    return flightValidator;
                });

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Booking Service API",
                    Description = "Simple API to perform CRUDs on Flight Bookings",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.DescribeAllParametersInCamelCase();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Service API V1");
            });

            app.UseMvc();
        }
    }
}
