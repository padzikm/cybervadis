namespace Foosball.Application.GetGame

open System
open Foosball.Application.Database
open Foosball.Application.Errors
open MediatR

module Types =
    
    type SetDto = {
        SetNr: int
        FirstTeamGoals: int
        SecondTeamGoals: int
    }
    
    type GetGameQueryResult = {
        GameId: Guid
        FirstTeamId: Guid
        SecondTeamId: Guid
        WinnerId: Nullable<Guid>
        Sets: SetDto[]
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
    open Foosball.Domain.ComplexTypes
    open Foosball.Domain.Logic
    
    let mapToDto (g: FoosballGame) =
        match g with
        | CompletedFoosballGame (d, r) ->
            let r:GetGameQueryResult = {
                GameId = GameId.value d.Id
                FirstTeamId = TeamId.value d.FirstTeamId
                SecondTeamId = TeamId.value d.SecondTeamId
                WinnerId = Nullable<Guid>()
                Sets = Array.empty
            }
            r
        | OngoingFoosballGame (d, r) ->
            let r:GetGameQueryResult = {
                GameId = GameId.value d.Id
                FirstTeamId = TeamId.value d.FirstTeamId
                SecondTeamId = TeamId.value d.SecondTeamId
                WinnerId = Nullable<Guid>()
                Sets = Array.empty
            }
            r
    
    type GetGameHandler(repo: IRepository) =
        inherit RequestHandler<GetGameQuery, Async<Result<GetGameQueryResult, Error>>>()

        override this.Handle(request) =
            async{
                let! r = repo.GetGame request.GameId
                return Ok (mapToDto r)
            }

