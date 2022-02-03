module Foosball.Domain.ComplexTypes

open System
open Foosball.Domain.SimpleTypes

type OngoingSetDetails =
    {
        FirstTeamId: TeamId
        FirstTeamGoals: int
        SecondTeamId: TeamId
        SecondTeamGoals: int
    }
    
type CompletedSetDetails =
    {
        WinnerTeamId: TeamId
        LooserTeamId: TeamId
        LooserTeamGoals: int
    }

type OngoingFoosballGameResult =
    | ZeroToZero of OngoingSetDetails
    | OneToZero of CompletedSetDetails * OngoingSetDetails
    | OneToOne of CompletedSetDetails * CompletedSetDetails * OngoingSetDetails

type CompletedFoosballGameResult =
    | TwoToZero of CompletedSetDetails * CompletedSetDetails * TeamId
    | TwoToOne of CompletedSetDetails * CompletedSetDetails * CompletedSetDetails * TeamId
    
type FoosballGameDetails =
    {
        Id: GameId
        FirstTeamId: TeamId
        FirstTeamName: TeamName
        SecondTeamId: TeamId
        SecondTeamName: TeamName
        StartDate: DateTime
    }
    
type FoosballGame =
    | OngoingFoosballGame of FoosballGameDetails * OngoingFoosballGameResult
    | CompletedFoosballGame of FoosballGameDetails * CompletedFoosballGameResult