module Foosball.Domain.Logic

open System
open Foosball.Domain.ComplexTypes
open Foosball.Domain.DomainEvents
open Foosball.Domain.SimpleTypes
open Foosball.Domain.Errors
open FsToolkit.ErrorHandling

//should include results and validation
let createFoosballGame (d: {| id: GameId; dt: DateTime; t1id: TeamId; t1n: TeamName; t2id: TeamId; t2n: TeamName |}) =
    let sd: OngoingSetDetails = {
        FirstTeamId = d.t1id; FirstTeamGoals = 0; SecondTeamId = d.t2id; SecondTeamGoals = 0 
    }
    let gd: FoosballGameDetails = {
        Id = d.id; StartDate = d.dt; FirstTeamId = d.t1id; FirstTeamName = d.t1n; SecondTeamId = d.t2id; SecondTeamName = d.t2n
    }
    let gr = ZeroToZero sd
    OngoingFoosballGame (gd, gr)
    

type private FoosballGameResult =
    | OngoingGame of OngoingFoosballGameResult
    | CompletedGame of CompletedFoosballGameResult
    
let private winningGoals = 10

// should be cleaner and using functional constructs, not ifs
let private scoreFirstTeam (setNr: int) (gr: OngoingFoosballGameResult) =
    match gr with
    | ZeroToZero r when setNr = 1 ->
        if r.FirstTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (ZeroToZero {r with FirstTeamGoals = r.FirstTeamGoals + 1}))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.FirstTeamId; LooserTeamId = r.SecondTeamId; LooserTeamGoals = r.SecondTeamGoals}
            let ns: OngoingSetDetails = {FirstTeamId = r.FirstTeamId; FirstTeamGoals = 0; SecondTeamId = r.SecondTeamId; SecondTeamGoals = 0}
            Ok (OngoingGame (OneToZero (fw, ns)))
    | OneToZero (cfr, r) when setNr = 2 ->
        if r.FirstTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (OneToZero (cfr, {r with FirstTeamGoals = r.FirstTeamGoals + 1})))
        elif cfr.WinnerTeamId = r.FirstTeamId then
            let fw: CompletedSetDetails = {WinnerTeamId = r.FirstTeamId; LooserTeamId = r.SecondTeamId; LooserTeamGoals = r.SecondTeamGoals}
            Ok (CompletedGame (TwoToZero (cfr, fw, r.FirstTeamId)))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.FirstTeamId; LooserTeamId = r.SecondTeamId; LooserTeamGoals = r.SecondTeamGoals}
            let ns: OngoingSetDetails = {FirstTeamId = r.FirstTeamId; FirstTeamGoals = 0; SecondTeamId = r.SecondTeamId; SecondTeamGoals = 0}
            Ok (OngoingGame (OneToOne (cfr, fw, ns)))
    | OneToOne (cfr, csr, r) when setNr = 3 ->
        if r.FirstTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (OneToOne (cfr, csr, {r with FirstTeamGoals = r.FirstTeamGoals + 1})))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.FirstTeamId; LooserTeamId = r.SecondTeamId; LooserTeamGoals = r.SecondTeamGoals}
            Ok (CompletedGame (TwoToOne (cfr, csr, fw, r.FirstTeamId)))
    | _ -> Error CannotScoreOutOfOrder
    
let private scoreSecondTeam (setNr: int) (gr: OngoingFoosballGameResult) =
    match gr with
    | ZeroToZero r when setNr = 1 ->
        if r.SecondTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (ZeroToZero {r with SecondTeamGoals = r.SecondTeamGoals + 1}))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.SecondTeamId; LooserTeamId = r.FirstTeamId; LooserTeamGoals = r.FirstTeamGoals}
            let ns: OngoingSetDetails = {FirstTeamId = r.FirstTeamId; FirstTeamGoals = 0; SecondTeamId = r.SecondTeamId; SecondTeamGoals = 0}
            Ok (OngoingGame (OneToZero (fw, ns)))
    | OneToZero (cfr, r) when setNr = 2 ->
        if r.SecondTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (OneToZero (cfr, {r with SecondTeamGoals = r.SecondTeamGoals + 1})))
        elif cfr.WinnerTeamId = r.SecondTeamId then
            let fw: CompletedSetDetails = {WinnerTeamId = r.SecondTeamId; LooserTeamId = r.FirstTeamId; LooserTeamGoals = r.FirstTeamGoals}
            Ok (CompletedGame (TwoToZero (cfr, fw, r.SecondTeamId)))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.SecondTeamId; LooserTeamId = r.FirstTeamId; LooserTeamGoals = r.FirstTeamGoals}
            let ns: OngoingSetDetails = {FirstTeamId = r.FirstTeamId; FirstTeamGoals = 0; SecondTeamId = r.SecondTeamId; SecondTeamGoals = 0}
            Ok (OngoingGame (OneToOne (cfr, fw, ns)))
    | OneToOne (cfr, csr, r) when setNr = 3 ->
        if r.SecondTeamGoals + 1 < winningGoals then
            Ok (OngoingGame (OneToOne (cfr, csr, {r with SecondTeamGoals = r.SecondTeamGoals + 1})))
        else
            let fw: CompletedSetDetails = {WinnerTeamId = r.SecondTeamId; LooserTeamId = r.FirstTeamId; LooserTeamGoals = r.FirstTeamGoals}
            Ok (CompletedGame (TwoToOne (cfr, csr, fw, r.SecondTeamId)))
    | _ -> Error CannotScoreOutOfOrder
    
let private updateGame (gd: FoosballGameDetails) (gr: FoosballGameResult) =
    match gr with
    | OngoingGame o ->
        OngoingFoosballGame (gd, o)
    | CompletedGame c ->
        CompletedFoosballGame (gd, c)

// should be better state machine using functional constructs
let scoreGoal (fg: FoosballGame) (setNr: int) (tid: TeamId) =
    match fg with
    | CompletedFoosballGame _ -> Error CannotScoreInCompletedGame
    | OngoingFoosballGame (d, r) when d.FirstTeamId = tid ->
        result{
            let! nr = r |> scoreFirstTeam setNr
            let ng = nr |> updateGame d
            let e: GoalScoredDomainEvent = {
                GameId = d.Id
                TeamId = tid
                SetNr = setNr
            }
            return (ng, e)
        }
    | OngoingFoosballGame (d, r) when d.SecondTeamId = tid ->
        result{
            let! nr = r |> scoreSecondTeam setNr
            let ng = nr |> updateGame d
            let e: GoalScoredDomainEvent = {
                GameId = d.Id
                TeamId = tid
                SetNr = setNr
            }
            return (ng, e)
        }
    | _ -> Error TeamNotPlayingInGame
        