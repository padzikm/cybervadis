namespace Foosball.Application.Database

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Linq
open Foosball.Domain.ComplexTypes
open Foosball.Domain.SimpleTypes

//should include error handling and optionally use interpreters
type IRepository =
    abstract member SaveGame: FoosballGame -> Async<unit>
    abstract member GetGame: Guid -> Async<FoosballGame>
    abstract member GetGames: DateTime -> Async<FoosballGame list>
    