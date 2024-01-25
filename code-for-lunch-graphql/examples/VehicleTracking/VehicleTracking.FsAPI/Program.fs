open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open VehicleTracking.FsAPI.Queries
open VehicleTracking.FsAPI.Vehicles.Queries


let builder = WebApplication.CreateBuilder();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddType<VehiclesQuery>()
    |> ignore
    
    
    
let app = builder.Build()
app.MapGraphQL() |> ignore
app.Run()
    
