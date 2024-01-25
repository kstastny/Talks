module VehicleTracking.Core.Domain

open System



type Person = {
    Id: Guid
    Name: string
    Surname: string
}


type Vehicle = {
    Id: Guid
    RegistrationPlate: string
    Label: string
    RootDriver: Person option
}