module Year2024Day06

open Common
open Extensions
open FSharp.Stats
open System.Collections.Generic



let getGuardPos m =
    m
    |> Array2D.mapi (fun row col c ->
        match c with
        | '^' -> Some(vector [| row; col |], vector [| -1; 0 |])
        | '>' -> Some(vector [| row; col |], vector [| 0; 1 |])
        | 'v' -> Some(vector [| row; col |], vector [| 1; 0 |])
        | '<' -> Some(vector [| row; col |], vector [| 0; -1 |])
        | _ -> None)
    |> Array2D.toSeq
    |> Seq.choose id
    |> Seq.head


let charAt m (v: Vector<float>) =
    let row, col = mapToTuple int v

    if outOfBounds m (row, col) then
        None
    else
        Some m[row, col]

let findNext m pos vel =
    let rec inner acc pos vel =
        match (charAt m (pos + vel)) with
        | Some '#' when acc < 4 -> inner (acc + 1) pos (rotate90 vel)
        | _ -> pos + vel, vel

    inner 0 pos vel

type GuardState =
    | Patrolling of ((int * int) * (int * int))
    | ExitedArea

let patrol m =
    let rec inner pos vel =
        seq {
            match charAt m pos with
            | None -> yield ExitedArea
            | Some _ ->
                yield Patrolling(mapToTuple int pos, mapToTuple int vel)
                let pos', vel' = findNext m pos vel
                yield! inner pos' vel'
        }

    let pos, vel = getGuardPos m

    inner pos vel

let tryObstruction m row col =
    let m' = Array2D.copy m
    m'[row, col] <- '#'

    let seen = HashSet<(int * int) * (int * int)>()

    patrol m'
    |> Seq.skipWhile (function
        | Patrolling p when seen.Contains(p) -> false
        | Patrolling p ->
            seen.Add(p) |> ignore
            true
        | ExitedArea -> false)
    |> Seq.head
    |> (function
    | Patrolling _ -> true
    | ExitedArea -> false)


let findCandidatesForObstruction m =
    let guardPos, _ = getGuardPos m
    let gRow, gCol = mapToTuple int guardPos

    let candidates =
        patrol m
        |> Seq.choose (function
            | Patrolling ((row, col), _) when (row, col) = (gRow, gCol) -> None
            | Patrolling ((row, col), _) -> Some(row, col)
            | ExitedArea -> None)
        |> Seq.distinct

    candidates
    |> Seq.map (fun (row, col) -> tryObstruction m row col)
    |> Seq.filter id
    |> Seq.length

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let m = parseToArray2D input

            patrol m
            |> Seq.filter (fun state ->
                match state with
                | Patrolling _ -> true
                | _ -> false)
            |> Seq.distinct
            |> Seq.length
            |> box


        member this.PartTwo(input: string) : obj =
            let m = parseToArray2D input

            findCandidatesForObstruction m |> box
