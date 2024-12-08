module Year2024Day08

open Common
open Extensions
open FSharp.Stats

let getAntennas m =
    m
    |> Array2D.mapi (fun row col freq -> freq, (row, col))
    |> Array2D.toSeq
    |> Seq.filter (fun (freq, _) -> freq <> '.')
    |> Seq.groupBy (fst)
    |> Seq.map (fun (freq, vals) ->
        freq,
        vals
        |> Seq.toList
        |> List.map (fun (_, (row, col)) -> vector [| row; col |]))

let getAntinodesFor m (antennas: Vector<float> list) =
    antennas
    |> List.allUnorderedPairs
    |> List.collect (fun (a, b) ->
        let delta = b - a
        [ a - delta; b + delta ])
    |> List.filter (fun pos -> not (outOfBoundsVec m pos))

let getResonantAntinodesFor m (antennas: Vector<float> list) =
    antennas
    |> List.allUnorderedPairs
    |> List.collect (fun (a, b) ->

        let delta = b - a

        let allInDirection start op =
            Seq.initInfinite (fun i -> op start (delta * (float) i))
            |> Seq.takeWhile (fun pos -> not (outOfBoundsVec m pos))

        Seq.append (allInDirection b (+)) (allInDirection a (-))
        |> Seq.toList)


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let m = parseToArray2D input

            getAntennas m
            |> Seq.collect (fun (_, positions) -> getAntinodesFor m positions)
            |> Seq.distinct
            |> Seq.length
            |> box

        member this.PartTwo(input: string) : obj =
            let m = parseToArray2D input
            let antennas = getAntennas m

            antennas
            |> Seq.collect (fun (_, positions) -> getResonantAntinodesFor m positions)
            |> Seq.distinct
            |> Seq.length
            |> box
