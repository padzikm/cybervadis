namespace Foosball.Application.Database

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Linq
open Foosball.Application.Database
open Foosball.Domain.ComplexTypes
open Foosball.Domain.SimpleTypes
    
type Repository() =
    static member Games = Dictionary<Guid, FoosballGame>()
    
    interface IRepository with
        member this.GetGame(id) =
            async{
                return Repository.Games.[id]
            }
            
        member this.GetGames(dt) =
            async{
                let l = Repository.Games.Where(fun g ->
                        match g.Value with
                        | CompletedFoosballGame (d, cg) ->
                            d.StartDate >= dt
                        | OngoingFoosballGame (d, cg) ->
                            d.StartDate >= dt
                        ).Select(fun p -> p.Value)
                return List.ofSeq l
            }
        member this.SaveGame(g) =
            async{
                return match g with
                    | CompletedFoosballGame (d, cg) ->
                        let id = GameId.value d.Id
                        Repository.Games.[id] = g |> ignore
                    | OngoingFoosballGame (d, cg) ->
                        let id = GameId.value d.Id
                        Repository.Games.[id] = g |> ignore
            }