module VehicleTracking.Core.Storage

open System
open System.Threading
open VehicleTracking.Core.Domain

module private MockData =
    
    let person name surname =
        {
            Id = Guid.NewGuid()
            Name = name
            Surname = surname 
        }
        
        
    let drivers =
        [
            person "Roman" "Provazník"
            person "Karel" "Šťastný"
            person "Petr" "Pavel"
            person "Pavel" "Petr"
            person "Josef" "Nowak"
            person "Lev" "Prchala"
            person "Alexandra" "Ukrutná"
            person "Viktorie" "Vítězná"
            person "Anastázie" "Vavrochová"
            person "Marie" "Radhouzká"
            person "Vlasta" "Machová"
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
        
        
let getDrivers (_: CancellationToken) = task { return MockData.drivers }
    
    
   
let getVehicles (_: CancellationToken) = task { return MockData.vehicles }