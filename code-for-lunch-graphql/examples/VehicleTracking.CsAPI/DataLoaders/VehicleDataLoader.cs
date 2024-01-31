using VehicleTracking.Core;

namespace VehicleTracking.CsAPI.DataLoaders;

public class VehicleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options,
    Storage.Storage storage)
    : BatchDataLoader<Guid, Domain.Vehicle>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<Guid, Domain.Vehicle>>
        LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var drivers = await storage.GetVehicles(cancellationToken);

        return drivers
            .ToDictionary(x => x.Id);
    }
}