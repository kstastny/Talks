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
        var Vehicles = await storage.GetVehicles(cancellationToken);

        return Vehicles.Select(VehicleOutput.OfVehicle).ToArray();
    }
}