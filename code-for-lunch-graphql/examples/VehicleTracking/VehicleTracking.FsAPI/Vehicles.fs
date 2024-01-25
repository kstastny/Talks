module VehicleTracking.FsAPI.Vehicles

open System
open System.Collections.Generic
open System.Collections.ObjectModel

open System.ComponentModel.DataAnnotations
open System.Threading
open System.Threading.Tasks
open HotChocolate
open HotChocolate.Types


open VehicleTracking.Core.Domain
open VehicleTracking.Core.Storage

open VehicleTracking.FsAPI.DataLoaders
open VehicleTracking.FsAPI.RootObjects


module InputTypes =
    
    type VehicleInput() =
        
        [<Required>]
        member val Id = Guid.Empty with get, set
        
        
        [<Required>]
        [<GraphQLDescription("Label for easier identification of vehicle")>]
        member val Label = "" with get, set
    
    
    
module OutputTypes =
    

    type VehicleOutput() =
        
        member val Id = Guid.Empty with get, set
        
        [<GraphQLDescription("Currently assigned registration plate")>]
        member val RegistrationPlate = "" with get, set
    
        [<GraphQLDescription("Label for easier identification of vehicle")>]
        member val Label = "" with get, set
        
        member val RootDriverId : Nullable<Guid> = Unchecked.defaultof<_> with get, set
        
        member x.GetRootDriverNaive(
            [<Service>] storage: Storage,
            [<Parent>] parent: VehicleOutput,
            cancellationToken: CancellationToken
            ) =
            task {
                if parent.RootDriverId.HasValue then
                    let! driver = storage.GetDriverById(parent.RootDriverId.Value, cancellationToken)
                    return driver |> DriverOutput.ofDriver
                else
                    return Unchecked.defaultof<_>
            }
            
            
        member x.GetRootDriverDataLoader(
            dataLoader: DriverDataLoader,
            [<Parent>] parent: VehicleOutput,
            cancellationToken: CancellationToken
            ) =
            task {
                if parent.RootDriverId.HasValue then
                    let! driver = dataLoader.LoadAsync(parent.RootDriverId.Value, cancellationToken)
                    return driver |> Option.map DriverOutput.ofDriver |> Option.defaultValue Unchecked.defaultof<_>
                else
                    return Unchecked.defaultof<_>
            }
            
        static member ofVehicle(vehicle: Vehicle) =
            VehicleOutput(
                       Id = vehicle.Id,
                       RegistrationPlate = vehicle.RegistrationPlate,
                       Label = vehicle.Label,
                       RootDriverId = (match vehicle.RootDriver with
                                       | Some d -> d.Id |> Nullable
                                       | None -> Unchecked.defaultof<_>)
                       )            
            

    
    and DriverOutput() =
        
        member val Id = Guid.Empty with get, set
        
        member val Name = "" with get, set
        
        member val Surname = "" with get, set
        
        [<GraphQLIgnore>]
        member val VehicleIds : Guid array = [||] with get, set
        
        [<GraphQLDescription("Vehicles assigned to this driver. Bosses may have more than one :)")>]
        member x.GetVehicles(
            [<Service>] storage: Storage,
            dataLoader: VehicleDataLoader,
            [<Parent>] parent: DriverOutput,
            cancellationToken: CancellationToken
            ) =
            task {
                //NOTE: for the sake of example, we are only loading IDs here
                let! vehicleIds = storage.GetDriverVehicleIds(parent.Id, cancellationToken)
                
                //batch load all vehicles
                let! vehicles = dataLoader.LoadAsync(vehicleIds |> List |> ReadOnlyCollection, cancellationToken)  
                
                return vehicles |> Seq.choose id |> Seq.map VehicleOutput.ofVehicle |> Array.ofSeq
            }         
    
        static member ofDriver(driver: Driver) =
             DriverOutput(
                        Id = driver.Id,
                        Name = driver.Name,
                        Surname = driver.Surname
                        )
            
    
    
module Queries =
    
    open OutputTypes
    
    [<ExtendObjectType(nameof RootQuery)>]
    type VehiclesQuery() =
        
        
        [<GraphQLNonNullType(false, false)>]
        member x.GetVehicles(
            [<Service>] storage: Storage,
            cancellationToken: CancellationToken) : Task<VehicleOutput list> =
            task {
                let! vehicles = storage.GetVehicles cancellationToken
                
                return vehicles
                       |> List.map VehicleOutput.ofVehicle
                
            }
        


module Mutations =
    
    open InputTypes
    open OutputTypes
    
    [<ExtendObjectType(nameof RootMutation)>]
    type VehiclesMutation() =
        
        
        member x.UpdateLabel(
            [<Service>] storage: Storage,
            vehicleInput: VehicleInput,
            cancellationToken: CancellationToken) : Task<VehicleOutput> =
            task {
                let! updatedVehicle = storage.UpdateVehicleLabel(vehicleInput.Id, vehicleInput.Label, cancellationToken)
                
                return updatedVehicle |> VehicleOutput.ofVehicle
            }