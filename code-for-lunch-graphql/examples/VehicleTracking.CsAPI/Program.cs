﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VehicleTracking.Core;
using VehicleTracking.CsAPI.Drivers;
using VehicleTracking.CsAPI.Positions;
using VehicleTracking.CsAPI.RootObjects;
using VehicleTracking.CsAPI.Vehicles;


var builder = WebApplication.CreateBuilder();

// keep everything in memory
builder.Services.AddSingleton<Storage.Storage>();

builder.Services
        .AddGraphQLServer()
        .AddInMemorySubscriptions()
        //queries
        .AddQueryType<RootQuery>()
        .AddTypeExtension<VehiclesQuery>()
        .AddTypeExtension<DriversQuery>()
        //mutations
        .AddMutationType<RootMutation>()
        .AddTypeExtension<VehiclesMutation>()
        //add subscriptions    
        .AddSubscriptionType<RootSubscription>()
        .AddTypeExtension<PositionSubscription>()
        ;



var app = builder.Build();
app.MapGraphQL();
app.UseWebSockets();
app.Run();