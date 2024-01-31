using VehicleTracking.Core;

namespace VehicleTracking.CsAPI.DataLoaders;

public class DriverDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options,
    Storage.Storage storage)
    : BatchDataLoader<Guid, Domain.Driver>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<Guid, Domain.Driver>>
        LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var drivers = await storage.GetDrivers(cancellationToken);

        return drivers
            .ToDictionary(x => x.Id);
    }
}