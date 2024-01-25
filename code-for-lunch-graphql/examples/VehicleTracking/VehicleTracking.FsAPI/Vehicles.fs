module VehicleTracking.FsAPI.Vehicles

open System
open System.ComponentModel.DataAnnotations

open System.Threading
open System.Threading.Tasks
open HotChocolate
open HotChocolate.Types

open VehicleTracking.Core.Storage

open VehicleTracking.FsAPI.Queries


module InputTypes =
    
    ()
    
    
    
module OutputTypes =
    

    type VehicleOutput() =
        
        member val Id = Guid.Empty with get, set
        
        [<GraphQLDescription("Currently assigned registration plate")>]
        member val RegistrationPlate = "" with get, set
    
        [<GraphQLDescription("Label for easier identification of vehicle")>]
        member val Label = "" with get, set
        
        member val RootDriverId : Nullable<Guid> = Unchecked.defaultof<_> with get, set
        
        //TODO lazy load using DataLoader - can be done as extension method?
        // [<GraphQLDescription("Person responsible for the vehicle, they have it assigned and use it on regular basis")>]
        // member val RootDriver : Driver = Unchecked.defaultof<_> with get, set
        
    
    and DriverOutput() =
        
        member val Id = Guid.Empty with get, set
        
        member val Name = Guid.Empty with get, set
        
        member val Surname = Guid.Empty with get, set

        //TODO        
        // [<GraphQLDescription("Vehicles assigned to this driver. Bosses may have more than one :)")>]
        // member val Vehicles : VehicleOutput = Unchecked.defaultof<_> with get, set
    
    
    
    
    
module Queries =
    
    open OutputTypes
    
    [<ExtendObjectType(nameof RootQuery)>]
    type VehiclesQuery() =
        
        //TODO show dependency injection
        [<GraphQLNonNullType(false, false)>]
        member x.GetVehicles(cancellationToken: CancellationToken) : Task<VehicleOutput list> =
            task {
                let! vehicles = getVehicles cancellationToken
                
                return vehicles
                       |> List.map (fun x ->
                           VehicleOutput(
                               Id = x.Id,
                               RegistrationPlate = x.RegistrationPlate,
                               Label = x.Label,
                               RootDriverId = (match x.RootDriver with
                                               | Some d -> d.Id |> Nullable
                                               | None -> Unchecked.defaultof<_>)
                               )
                           )
                
            }
        
