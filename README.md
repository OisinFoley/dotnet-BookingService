# Flight Booking Service

## .Net Core Web API Application for adding flight bookings to a SQL Azure database

## Features

- Perform CRUD operations to add new bookings for a flight
- Use of Azure Service Bus to communicate the addition of new bookings which subscribers can listen out for (namely, a separate flights service)
- Use of Repository Pattern, Unit of Work Pattern
- Use of DTOs to carry data between application layers
- Singleton Pattern for OutboundMessage service which forms Service Bus to Azure
- Swagger via Swashbuckle for quick API interaction
- Autorest-generated client to remove overhead when communicating with flights service API, which share related data

## Requirements

- Add your own appsettings.json file, with the following keys:

    ```
    "ConnectionStrings": {
        "SQLServer": "<something-here>"
    },
    "OutboundMessageService": {
        "ConnectionString": "<something-here>",
        "TopicName": "<something-here>",
        "PollingIntervalInMilliseconds": <something-here>
    },
    "FlightServiceClient": {
        "BaseUri": "http://localhost:5000"
    }
    ```

- Dotnet Core 2.1 [Link to microsoft downloads website](https://dotnet.microsoft.com/download)
- .NET SDK [Link to microsoft downloads website](https://dotnet.microsoft.com/download)
- Add a reference to the EventDispatcher NuGet used in this app by the service bus **
- Access to the flight service API client used by the app **

---

** If you're a random user just passing by this repo, then i'll need to send you these.

** If i've sent you the contents of this repo, but not these additional NuGets, please email me at oisinfoley@yahoo.co.uk and i'll forward them to you.

