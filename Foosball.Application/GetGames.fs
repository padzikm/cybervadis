namespace Foosball.Application.GetGames

open System
open Foosball.Application.Database
open Foosball.Application.Errors
open MediatR

module Types =
    
    type GetGamesQueryResult = {
        Games: Foosball.Domain.ComplexTypes.FoosballGame list
    }
    
    type GetGamesQuery(startDt: DateTime) =
        let guid = Guid.NewGuid()
        let dt = DateTime.UtcNow
        
        member _.CommandId = guid
        member _.CommandDateTime = dt
        member val StartDt = startDt with get
        
        interface IRequest<Async<Result<GetGamesQueryResult, Error>>> with
        
module Implementation =
    open Types
    open Foosball.Domain.SimpleTypes
    open Foosball.Domain.Logic
    
    type GetGamesHandler(repo: IRepository) =
        inherit RequestHandler<GetGamesQuery, Async<Result<GetGamesQueryResult, Error>>>()

        override this.Handle(request) =
            async{
                let! r = repo.GetGames request.StartDt
                return Ok {Games = r}
            }
