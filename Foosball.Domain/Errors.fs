namespace Foosball.Domain.Errors

type StringError =
    | Null
    | Empty
    | MaxLengthExceeded of int
    | ContainsNewline

type ValidationError =
    | StringError of StringError
    
type DomainError =
    | TeamNotPlayingInGame
    | CannotScoreInCompletedGame
    | CannotScoreOutOfOrder