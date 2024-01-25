module VehicleTracking.FsAPI.Positions

open System
open System.Threading
open HotChocolate
open HotChocolate.Subscriptions
open HotChocolate.Types
open VehicleTracking.Core.Storage
open VehicleTracking.FsAPI.RootObjects

open FSharp.Control


type VehiclePosition() =
    
    member val Latitude : float = 0.0 with get, set
    
    member val Longitude : float = 0.0 with get, set
    
    member val VehicleId : Guid = Guid.Empty with get, set 



[<ExtendObjectType(nameof RootSubscription)>]
type PositionSubscription(
    [<Service>] storage: Storage,
    [<Service>] topicEventSender: ITopicEventSender
    ) =
    
    let random = Random()
    let nextPosition =
        
        let minLat = 44.8948358
        let maxLat = 52.5841061
        let minLon = 4.9568123
        let maxLon = 28.1767924
        
        fun vehicleId ->
            VehiclePosition(
                VehicleId = vehicleId,
                Latitude = random.NextDouble()*(maxLat - minLat) + minLat,
                Longitude = random.NextDouble()*(maxLon - minLon) + minLon
                )
        
    
    let topicName = "vehiclePositions"

    let sendPosition () = task {
        let! vehicle = storage.GetVehicles(CancellationToken.None)
                       |> Task.map List.head
                       
        do! topicEventSender.SendAsync(topicName, nextPosition vehicle.Id, CancellationToken.None)
    }    

    let _ = new Timer(
              (fun _ -> sendPosition () |> ignore),
              null,
              (TimeSpan.FromSeconds 5.0),
              (TimeSpan.FromSeconds 5.0))

   
    
    member x.SubscribeToPositionStream
        (
            [<Service>] receiver: ITopicEventReceiver,
            cancellationToken: CancellationToken
        ) =
        taskSeq {
            //during subscription, we can send previously known data and then connect
            let! vehicle = storage.GetVehicles(cancellationToken) |> Task.map List.head 
            
            yield nextPosition vehicle.Id
            
            //NOTE: the topic can be dynamic, e.g. for a specific car, user, etc.
            let! items = receiver.SubscribeAsync<VehiclePosition>(topicName, cancellationToken)

            yield! items.ReadEventsAsync()
        }


    [<Subscribe(With = "SubscribeToPositionStream")>]
    member x.OnPositionMessage
        (
            [<EventMessage>] message: VehiclePosition
        ) =
        message
        