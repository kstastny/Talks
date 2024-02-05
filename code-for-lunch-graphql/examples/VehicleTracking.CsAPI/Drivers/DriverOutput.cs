using VehicleTracking.Core;
using VehicleTracking.CsAPI.DataLoaders;
using VehicleTracking.CsAPI.Vehicles;


namespace VehicleTracking.CsAPI.Drivers;

public record DriverOutput
{
    public DriverOutput(Guid id, string name, string surname)
    {
        Id = id;
        Name = name;
        Surname = surname;
    }

    [GraphQLDescription("Driver identifier")]
    public Guid Id { get; init; }

    [GraphQLDescription("Driver name")]
    public string Name { get; init; }

    [GraphQLDescription("Driver surname")]
    public string Surname { get; init; }


    [GraphQLDescription("Vehicles assigned to this driver. Bosses may have more than one :)")]
    public async Task<VehicleOutput[]> GetVehicles(
        [Service] Storage.Storage storage,
        VehicleDataLoader dataLoader,
        [Parent] DriverOutput parent,
        CancellationToken cancellationToken)
    {
        // NOTE: for the sake of example, we are only loading IDs here
        var vehicleIds = await storage.GetDriverVehicleIds(parent.Id, cancellationToken);
        
        // batch load all vehicles
        var vehicles = 
            await dataLoader.LoadAsync(vehicleIds.ToList().AsReadOnly(), cancellationToken);
        
        return vehicles.Select(VehicleOutput.OfVehicle).ToArray();
    }

    public static DriverOutput OfDriver(Domain.Driver driver)
    {
        return new DriverOutput(driver.Id, driver.Name, driver.Surname);
    }
}