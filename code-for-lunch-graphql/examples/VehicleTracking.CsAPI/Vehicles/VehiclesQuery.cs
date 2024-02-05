using VehicleTracking.Core;
using VehicleTracking.CsAPI.RootObjects;

namespace VehicleTracking.CsAPI.Vehicles;

[ExtendObjectType<RootQuery>]
public class VehiclesQuery
{
    public async Task<VehicleOutput[]> GetVehicles(
        [Service] Storage.Storage storage,
        CancellationToken cancellationToken)
    {
        var vehicles = await storage.GetVehicles(cancellationToken);

        return vehicles.Select(VehicleOutput.OfVehicle).ToArray();
    }
}