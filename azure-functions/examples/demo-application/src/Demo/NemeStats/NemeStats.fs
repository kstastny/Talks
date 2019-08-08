namespace NemeStats.Import.NemeStats

open System

open NemeStats.Import.Date


// https://nemestatsapiversion2.docs.apiary.io/#reference

type AuthenticationToken =
    {
        Token: string;
        Expiration: DateTime;
    }

type GamingGroup =
    {
        Id: int
    }

type Player =
    {
        Id: int
        Name: string
    }
    
type GameDefinition =
    {
        Id: int
        Name: string
        BggId: int option
    }
    

// Player in played game
type PlayerRank =
    {
        Player: Player
        
        /// The corresponding rank of the Player in this Played Game. A rank of 1 means the Player got first place, 2 means second place, and so on.
        GameRank: int
        
        PointsScored: int option
    }
    
type PlayedGame =
    {
        Id: int
        GameDefinition: GameDefinition
        Notes: string
        DatePlayed: Date
        BggPlayId: int option
        PlayerRanks: PlayerRank list
    }

