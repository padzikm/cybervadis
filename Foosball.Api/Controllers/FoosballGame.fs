namespace Foosball.Api.Controllers

open System

open Foosball.Application.CreateGame.Types
open Foosball.Application.GetGame.Types
open Foosball.Application.GetGames.Types
open Foosball.Application.ScoreGoal.Types
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.AsyncResult
open FSharpPlus
open MediatR
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Foosball.Application.Errors

[<CLIMutable>]
type CreateGameRequest = {
    FirstTeamName: string
    SecondTeamName: string
}

[<CLIMutable>]
type ScoreGoalRequest = {
    TeamId: Guid
    SetNr: int
}

[<Route("api/[controller]")>]
[<ApiController>]
type FoosballGameController (logger: ILogger<FoosballGameController>, dispatch: IMediator) as this =
    inherit ControllerBase()
    
    let send (dispatch: IMediator) cmd =
        let r = AsyncResult.ofTask (dispatch.Send cmd)
        let y = r |> AsyncResult.foldResult id (InfrastructureError >> Error >> Async.singleton)
        let q = y |> Async.join
        q
            
    [<HttpPost>]
    member this.CreateGame(req: CreateGameRequest) =
        let cmd = CreateGameCommand(req.FirstTeamName, req.SecondTeamName)

        let q = send dispatch cmd
        let u = q |> AsyncResult.foldResult (fun x -> this.Ok x :> ActionResult) (fun x -> this.BadRequest() :> ActionResult)
        u
        
    //should be splitted to async process with redirecting to task resource for waiting for result
    [<HttpPost("game/{id}/score")>]
    member this.ScoreGoal(id: Guid, req: ScoreGoalRequest) =
        let cmd = ScoreGoalCommandRequest(id, req.TeamId, req.SetNr)

        let q = send dispatch cmd
        let u = q |> AsyncResult.foldResult (fun x -> this.Ok x :> ActionResult) (fun x -> this.BadRequest() :> ActionResult)
        u

    [<HttpGet("game/{id}")>]
    member this.GetGame(id: Guid) =
        let q = GetGameQuery(id)
        
        let q = send dispatch q
        let u = q |> AsyncResult.foldResult (fun x -> this.Ok x :> ActionResult) (fun x -> this.BadRequest() :> ActionResult)
        u
        
    [<HttpGet("games/{dt}")>]
    member this.GetGames(dt: DateTime) =
        let q = GetGamesQuery(dt)
        
        let q = send dispatch q
        let u = q |> AsyncResult.foldResult (fun x -> this.Ok x :> ActionResult) (fun x -> this.BadRequest() :> ActionResult)
        u

