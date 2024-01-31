using VehicleTracking.Core;
using VehicleTracking.CsAPI.RootObjects;

namespace VehicleTracking.CsAPI.Drivers;

[ExtendObjectType<RootQuery>]
public class DriversQuery
{
    public async Task<DriverOutput[]> GetDrivers(
        [Service] Storage.Storage storage,
        CancellationToken cancellationToken)
    {
        var drivers = await storage.GetDrivers(cancellationToken);

        return drivers.Select(DriverOutput.OfDriver).ToArray();
    }
}