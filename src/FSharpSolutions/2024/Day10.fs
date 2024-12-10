module Year2024Day10

open Common
open Extensions
open FSharp.Stats


let directions =
    [ vector [| -1.; 0. |]
      vector [| 0.; 1. |]
      vector [| 1.; 0. |]
      vector [| 0.; -1. |] ]

let valAt m pos =
    let row, col = mapToTuple int pos

    if outOfBounds m (row, col) then
        10
    else
        m[row, col]


let findTrailheadScore (m: int [,]) pos =
    let rec inner m pos =
        let curr = valAt m pos

        match curr with
        | 9 -> [ pos ]
        | n when n < 9 ->
            directions
            |> List.map ((+) pos)
            |> List.where (fun nextPos -> (valAt m nextPos) - curr = 1)
            |> List.collect (inner m)
            |> List.distinct
        | _ -> failwith "unexpected error"

    inner m pos

let findTrailheadScores (m: int [,]) =
    m
    |> Array2D.mapi (fun row col n -> n, vector [| row; col |])
    |> Array2D.toSeq
    |> Seq.filter (fun (n, _) -> n = 0)
    |> Seq.map (fun (n, pos) -> findTrailheadScore m pos |> List.length)
    |> Seq.sum

let toRowColString v =
    let row, col = mapToTuple int v
    $"({row},{col})"

let findTrailheadRating m pos =
    let rec inner acc m pos =
        let curr = valAt m pos
        let acc' = acc + toRowColString pos

        match curr with
        | 0 -> [ acc' ]
        | n when n < 10 ->
            directions
            |> List.map ((+) pos)
            |> List.where (fun nextPos -> curr - (valAt m nextPos) = 1)
            |> List.collect (inner acc' m)
            |> List.distinct
        | _ -> failwith "unexpected error"

    inner "" m pos |> List.length


let findTrailheadRatings m =
    m
    |> Array2D.mapi (fun row col n -> n, vector [| row; col |])
    |> Array2D.toSeq
    |> Seq.filter (fun (n, _) -> n = 9)
    |> Seq.map (fun (_, pos) -> findTrailheadRating m pos)
    |> Seq.sum


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            input
            |> parseToArray2D
            |> Array2D.map (fun c -> int c - int '0')
            |> findTrailheadScores
            |> box

        member this.PartTwo(input: string) : obj =
            parseToArray2D input
            |> Array2D.map (fun c -> int c - int '0')
            |> findTrailheadRatings
            |> box
