module VehicleTracking.Core.Domain

open System



type Driver = {
    Id: Guid
    Name: string
    Surname: string
}


type Vehicle = {
    Id: Guid
    RegistrationPlate: string
    Label: string
    RootDriver: Driver option
}