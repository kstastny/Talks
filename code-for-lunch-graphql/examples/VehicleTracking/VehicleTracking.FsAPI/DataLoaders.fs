module VehicleTracking.FsAPI.DataLoaders

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Threading
open System.Threading.Tasks
open GreenDonut
open VehicleTracking.Core.Domain
open VehicleTracking.Core.Storage

type DriverDataLoader(batchScheduler, options, storage: Storage) =
    inherit BatchDataLoader<Guid, Driver option>(batchScheduler, options)
    
    
        override this.LoadBatchAsync
        (
            _: IReadOnlyList<Guid>,
            cancellationToken: CancellationToken
        ) : Task<IReadOnlyDictionary<Guid, Driver option>> =
        task {
            //NOTE: for simplicity, we are loading everything
            let! drivers = storage.GetDrivers(cancellationToken)
            
            return
                drivers
                |> Seq.map (fun x -> x.Id, Some x)
                |> dict
                |> ReadOnlyDictionary
                :> IReadOnlyDictionary<Guid, Driver option>
        }
        
        

type VehicleDataLoader(batchScheduler, options, storage: Storage) =
    inherit BatchDataLoader<Guid, Vehicle option>(batchScheduler, options)
    
    
        override this.LoadBatchAsync
        (
            _: IReadOnlyList<Guid>,
            cancellationToken: CancellationToken
        ) : Task<IReadOnlyDictionary<Guid, Vehicle option>> =
        task {
            //NOTE: for simplicity, we are loading everything
            let! vehicles = storage.GetVehicles(cancellationToken)
            
            return
                vehicles
                |> Seq.map (fun x -> x.Id, Some x)
                |> dict
                |> ReadOnlyDictionary
                :> IReadOnlyDictionary<Guid, Vehicle option>
        }        