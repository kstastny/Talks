module VehicleTracking.Core.Storage

open System
open System.Collections.Concurrent
open System.Threading
open Microsoft.Extensions.Logging
open VehicleTracking.Core.Domain

module private MockData =
    
    let driver name surname =
        {
            Id = Guid.NewGuid()
            Name = name
            Surname = surname 
        }
        
        
    let drivers =
        [
            driver "Radoslav" "Vavroch"
            driver "Vlastimil" "Vypich"
            driver "Petr" "Pavel"
            driver "Pavel" "Petr"
            driver "Josef" "Nowak"
            driver "Lev" "Prchala"
            driver "Alexandra" "Ukrutná"
            driver "Viktorie" "Vítězná"
            driver "Anastázie" "Vavrochová"
            driver "Marie" "Radhouzká"
            driver "Vlasta" "Machová"
        ]
        
    let rand = Random()
    
    let getRandomDriver =
        let driverArray = drivers |> Array.ofList
        
        fun () ->
            driverArray[rand.Next(driverArray.Length)]
        
        

    let getRandomVehicleLabel =
        let makes =
            [|
                "Škoda"
                "VW"
                "Audi"
                "Toyota"
                "Honda"
                "Hyundai"
                "Ford"
                "DS"
                "Ssang Yong"
                "Renault"
                "Peugeot"
                "Lexus"
                "Mazda"
            |]
            
        let colors =
            [|
                "Red"
                "Green"
                "Black"
                "Beige"
                "Cappucino"
                "Grey"
                "Pink"
                "Yellow"
                "Octarine"
            |]
            
        fun () ->
            $"{colors[rand.Next(colors.Length)]} {makes[rand.Next(makes.Length)]}"
        
        
    
        
    let vehicle registrationPlate label =
        {
            Id = Guid.NewGuid()
            RegistrationPlate = registrationPlate
            Label = label
            RootDriver =
                match rand.NextDouble() with
                | x when x < 0.2 -> None
                | _ -> getRandomDriver() |> Some
        }
        
        
        
    let vehicles =
        [1..25]
        |> List.map (fun i ->
                vehicle $"1X1 %04d{i}" (getRandomVehicleLabel())
            )
        
        
        
type Storage(logger: ILogger<Storage>) =
    
    let vehicles =
        let vehiclesDict = ConcurrentDictionary<Guid, Vehicle>()
    
        MockData.vehicles
        |> List.iter (fun x -> vehiclesDict.TryAdd(x.Id, x) |> ignore)
        
        vehiclesDict
    
    
    member x.GetDrivers (_: CancellationToken) =
        task {
            logger.LogInformation("Calling Storage.GetDrivers")
            return MockData.drivers
        }
        
    member x.GetDriverVehicleIds (driverId: Guid, _: CancellationToken) =
        task {
            logger.LogInformation("Calling Storage.GetDriverVehicles")
            
            return
                vehicles.Values
                |> Seq.where (fun x -> x.RootDriver |> Option.map _.Id = Some driverId)
                |> Seq.map _.Id
                |> List.ofSeq
        }        
    
    member x.GetDriverById (driverId: Guid, _: CancellationToken) =
        task {
            logger.LogInformation("Calling Storage.GetDriverById")
            return MockData.drivers |> List.find (fun x -> x.Id = driverId)
        }
    

    member x.GetVehicles (_: CancellationToken) = task {
        logger.LogInformation("Calling Storage.GetVehicles")
        return vehicles.Values |> List.ofSeq
    }
    
    
    member x.UpdateVehicleLabel(vehicleId: Guid, label: string, _: CancellationToken) = task {
        let vehicle = vehicles[vehicleId]
        let updatedVehicle = { vehicle with Label = label }
        
        vehicles.AddOrUpdate(
            vehicleId,
            updatedVehicle,
            (fun k v -> updatedVehicle)
            )
        |> ignore
        
        return vehicle
    }