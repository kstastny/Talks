using FsToolkit.ErrorHandling.Operator.Option;
using Microsoft.FSharp.Core;
using VehicleTracking.Core;
using VehicleTracking.CsAPI.DataLoaders;
using VehicleTracking.CsAPI.Drivers;
using VehicleTracking.CsAPI.Positions;

namespace VehicleTracking.CsAPI.Vehicles;

public record VehicleOutput
{
    
    public VehicleOutput(Guid id, string registrationPlate, string label, Guid? rootDriverId)
    {
        Id = id;
        RegistrationPlate = registrationPlate;
        Label = label;
        RootDriverId = rootDriverId;
    }

    [GraphQLDescription("Vehicle identifier")]
    public Guid Id { get; init; }

    [GraphQLDescription("Currently assigned registration plate")]
    public string RegistrationPlate { get; init; }

    [GraphQLDescription("Label for easier identification of vehicle")]
    public string Label { get; init; }

    [GraphQLIgnore] public Guid? RootDriverId { get; init; }

    public async Task<VehiclePosition> GetPosition(
        [Parent] VehicleOutput parent,
        CancellationToken cancellationToken)
    {
        var positions = new Positions.Positions();
        await Task.Delay(1000, cancellationToken);
        return positions.NextPosition(parent.Id);
    }


    [GraphQLDescription("Gets the root driver naively.")]
    public async Task<DriverOutput> GetRootDriverNaive(
        [Service] Storage.Storage storage,
        [Parent] VehicleOutput parent,
        CancellationToken cancellationToken)
    {
        if (parent.RootDriverId.HasValue)
        {
            var driver = await storage.GetDriverById(parent.RootDriverId.Value, cancellationToken);
            return DriverOutput.OfDriver(driver);
        }
        else
        {
            return null;
        }
    }

    [GraphQLDescription("Gets the root driver using a data loader.")]
    public async Task<DriverOutput> GetRootDriverDataLoader(
        DriverDataLoader dataLoader,
        [Parent] VehicleOutput parent,
        CancellationToken cancellationToken)
    {
        if (parent.RootDriverId.HasValue)
        {
            var driver = await dataLoader.LoadAsync(parent.RootDriverId.Value, cancellationToken);
            return driver != null ? DriverOutput.OfDriver(driver) : null;
        }
        else
        {
            return default; // Or a more appropriate default value or error handling
        }
    }

    public static VehicleOutput OfVehicle(Domain.Vehicle vehicle)
    {
        return new VehicleOutput(
            vehicle.Id,
            vehicle.RegistrationPlate,
            vehicle.Label,
            FSharpOption<Domain.Driver>.get_IsSome(vehicle.RootDriver) ? vehicle.RootDriver.Value.Id : null);
    }
}