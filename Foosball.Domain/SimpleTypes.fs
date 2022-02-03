module Foosball.Domain.SimpleTypes

open System
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Validation
open Foosball.Domain.Errors

type TeamName = private TeamName of string

module TeamName =
    
    let create (s: string) =
//        let f _ = TeamName s
//        let r = s |> Result.requireNotNull Null |> Result.mapError StringError
//        f <!^> r
        TeamName s
        
    let map f (TeamName v) = TeamName (f v)
    
    let value (TeamName v) = v


type GameId = private GameId of Guid

module GameId =
    
    let create v =
        GameId v
        
    let value (GameId v) = v

type TeamId = private TeamId of Guid

module TeamId =
    
    let create v =
        TeamId v
        
    let value (TeamId v) = v

type NonNegativeInt = private NonNegativeInt of int

module NonNegativeInt =
    
    let create v =
        NonNegativeInt v
    
    let map f (NonNegativeInt v) = NonNegativeInt (f v)
    
    let value (NonNegativeInt v) = v
