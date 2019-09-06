module NemeStats.Import.Importer

open System.Threading

open FSharp.Control.Rop.TaskResult

open FSharp.Control.Rop
open NemeStats.Import.Date
open NemeStats.Import.BoardGameGeek
open NemeStats.Import.BoardGameGeek.Functions
open NemeStats.Import.NemeStats


type ImportParameters =
    {
        BggUsername: string
        DateFrom: Date
        DateTo: Date
    }
    with
        static member Create user datefrom dateto =
            {
                BggUsername = user
                DateFrom = datefrom
                DateTo = dateto
            }