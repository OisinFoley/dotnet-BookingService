# Flight Booking Service

## Dotnet Core Web API Application for adding flights to a SQL Azure database

## Features

- POST requests to add new flights
- Use of Azure Service Bus to communicate adding of new flights which subscribers can listen out for
- Repository Pattern, Unit of Work Pattern
- Singleton Pattern for OutboundMessage service which forms Service Bus to Azure
- Use of DTOs to carry data between layers

## Requirements

- Add your appsettings for 

 ```"ConnectionStrings": {```
```    "SQLServer": "<something-here>"
  },```
```  "OutboundMessageService": {
    "ConnectionString": "<something-here>", ```
```    "TopicName": "<something-here>",```
```    "PollingIntervalInMilliseconds": <something-here>
  }```

- Dotnet Core 2.1 [Link to microsoft downloads website](https://dotnet.microsoft.com/download)
- .NET SDK [Link to microsoft downloads website](https://dotnet.microsoft.com/download)
- Download the EventDispatcher library from this account too
- Add a reference to the EventDispatcher Nuget once downloaded