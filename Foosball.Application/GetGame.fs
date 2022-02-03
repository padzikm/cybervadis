namespace Foosball.Application.GetGame

open System
open Foosball.Application.Database
open Foosball.Application.Errors
open MediatR

module Types =
    
    type GetGameQueryResult = {
        Game: Foosball.Domain.ComplexTypes.FoosballGame
    }
    
    type GetGameQuery(gameId: Guid) =
        let guid = Guid.NewGuid()
        let dt = DateTime.UtcNow
        
        member _.CommandId = guid
        member _.CommandDateTime = dt
        member val GameId = gameId with get
        
        interface IRequest<Async<Result<GetGameQueryResult, Error>>> with
        
module Implementation =
    open Types
    open Foosball.Domain.SimpleTypes
    open Foosball.Domain.Logic
    
    type GetGameHandler(repo: IRepository) =
        inherit RequestHandler<GetGameQuery, Async<Result<GetGameQueryResult, Error>>>()

        override this.Handle(request) =
            async{
                let! r = repo.GetGame request.GameId
                return Ok {Game = r}
            }

