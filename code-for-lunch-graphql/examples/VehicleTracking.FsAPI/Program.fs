open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

open VehicleTracking.Core.Storage
open VehicleTracking.FsAPI.Positions
open VehicleTracking.FsAPI.RootObjects
open VehicleTracking.FsAPI.Vehicles.Mutations
open VehicleTracking.FsAPI.Vehicles.Queries


let builder = WebApplication.CreateBuilder()

// keep everything in memory
builder.Services.AddSingleton<Storage>() |> ignore

builder.Services
    .AddGraphQLServer()
    .AddInMemorySubscriptions()
    //queries
    .AddQueryType<RootQuery>()
    .AddTypeExtension<VehiclesQuery>()
    .AddTypeExtension<DriversQuery>()
    
    .AddMutationType<RootMutation>()
    .AddTypeExtension<VehiclesMutation>()
    //add subscriptions    
    .AddSubscriptionType<RootSubscription>()
    .AddTypeExtension<PositionSubscription>()
    |> ignore
    
    
    
let app = builder.Build()
app.MapGraphQL() |> ignore
app.UseWebSockets() |> ignore
app.Run()
    
