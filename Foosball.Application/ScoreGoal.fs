namespace Foosball.Application.ScoreGoal

open System
open System.Threading.Tasks
open Foosball.Application.Database
open Foosball.Application.Errors
open MediatR
open NServiceBus

module Types =
    
    type ScoreGoalCommandRequest(gameId: Guid, teamId: Guid, setNr: int) =
        let guid = Guid.NewGuid()
        let dt = DateTime.UtcNow
        
        member _.CommandId = guid
        member _.CommandDateTime = dt
        member val GameId = gameId with get
        member val TeamId = teamId with get
        member val SetNr = setNr with get
        
        interface IRequest<Async<Result<unit, Error>>> with
        
    [<CLIMutable>]
    type ScoreGoalCommand =
        {
            GameId: Guid
            DateTime: DateTime
            TeamId: Guid
            SetNr: int
        }
        interface ICommand with
        
module Implementation =
    open Types
    open Foosball.Domain.SimpleTypes
    open Foosball.Domain.Logic
    
    type ScoreGoalRequestHandler(session: IMessageSession) =
        inherit RequestHandler<ScoreGoalCommandRequest, Async<Result<unit, Error>>>()

        override this.Handle(request) =
            async{
                let cmd = {
                    GameId = request.GameId
                    TeamId = request.TeamId
                    SetNr = request.SetNr
                    DateTime = request.CommandDateTime
                }
                let! _ = session.Send cmd |> Async.AwaitTask
                return Ok ()
            }
    
    type ScoreGoalHandler(repo: IRepository) =
        interface IHandleMessages<ScoreGoalCommand> with
            member this.Handle(message, context) =
                async{
                    let! g = repo.GetGame message.GameId
                    let r = scoreGoal g message.SetNr (TeamId.create message.TeamId)
                    let! _ = match r with
                    | Ok (ng, e) ->
                        repo.SaveGame ng
                    | Error e -> failwith (sprintf "%O" e)
                    return ()
                } |> Async.StartAsTask :> Task