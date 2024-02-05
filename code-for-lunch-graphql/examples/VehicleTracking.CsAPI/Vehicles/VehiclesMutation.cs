using VehicleTracking.Core;
using VehicleTracking.CsAPI.RootObjects;

namespace VehicleTracking.CsAPI.Vehicles;

[ExtendObjectType<RootMutation>]
public class VehiclesMutation
{
    public async Task<VehicleOutput> UpdateLabel(
        [Service] Storage.Storage storage,
        VehicleInput vehicleInput,
        CancellationToken cancellationToken)
    {
        var updatedVehicle =
            await storage.UpdateVehicleLabel(vehicleInput.Id, vehicleInput.Label, cancellationToken);

        return VehicleOutput.OfVehicle(updatedVehicle);
    }
}