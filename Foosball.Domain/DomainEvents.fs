namespace Foosball.Domain.DomainEvents

open System
open Foosball.Domain.SimpleTypes

type FoosballGameCreatedDomainEvent = {
    GameId: GameId
    FirstTeamId: TeamId
    FirstTeamName: TeamName
    SecondTeamId: TeamId
    SecondTeamName: TeamName
}

type GoalScoredDomainEvent = {
    GameId: GameId
    TeamId: TeamId
    SetNr: int
}