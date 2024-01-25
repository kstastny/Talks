open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open VehicleTracking.Core
open VehicleTracking.Core.Storage
open VehicleTracking.FsAPI.Positions
open VehicleTracking.FsAPI.RootObjects
open VehicleTracking.FsAPI.Vehicles.Queries


let builder = WebApplication.CreateBuilder()

builder.Services.AddScoped<Storage>() |> ignore

builder.Services
    .AddGraphQLServer()
    .AddInMemorySubscriptions()
    .AddQueryType<RootQuery>()
    .AddType<VehiclesQuery>()
    //add subscriptions    
    .AddSubscriptionType<RootSubscription>()
    .AddTypeExtension<PositionSubscription>()
    |> ignore
    
    
    
let app = builder.Build()
app.MapGraphQL() |> ignore
app.UseWebSockets() |> ignore
app.Run()
    
