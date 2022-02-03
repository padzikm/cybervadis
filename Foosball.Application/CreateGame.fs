namespace Foosball.Application.CreateGame

open System
open Foosball.Application.Database
open Foosball.Application.Errors
open MediatR

module Types =
    
    type CreateGameCommandResult = {
        GameId: Guid
        FirstTeamId: Guid
        SecondTeamId: Guid
    }
    
    type CreateGameCommand(firstTeam: string, secondTeam: string) =
        let guid = Guid.NewGuid()
        let dt = DateTime.UtcNow
        
        member _.CommandId = guid
        member _.CommandDateTime = dt
        member val FirstTeam = firstTeam with get
        member val SecondTeam = secondTeam with get
        
        interface IRequest<Async<Result<CreateGameCommandResult, Error>>> with
        
module Implementation =
    open Types
    open Foosball.Domain.SimpleTypes
    open Foosball.Domain.Logic
    
    type CreateGameHandler(repo: IRepository) =
        inherit RequestHandler<CreateGameCommand, Async<Result<CreateGameCommandResult, Error>>>()

        override this.Handle(request) =
            async{
                let fn = TeamName.create request.FirstTeam
                let fid = TeamId.create (Guid.NewGuid())
                let sn = TeamName.create request.SecondTeam
                let sid = TeamId.create (Guid.NewGuid())
                let dt = DateTime.UtcNow
                let gid = GameId.create (Guid.NewGuid())
                let g = createFoosballGame {|id = gid; dt = dt; t1id = fid; t1n = fn; t2id = sid; t2n = sn|}
                let! _ = repo.SaveGame g
                let r = {
                    GameId = GameId.value gid
                    FirstTeamId = TeamId.value fid
                    SecondTeamId = TeamId.value sid
                }
                return Ok r
            }
