module Year2024Day04

open System
open Common
open Extensions
open FSharp.Stats

let parse (input: string) =
    let lines = input.Split('\n')

    Array2D.init lines.Length lines[0].Length (fun y x -> lines[y][x])

let directions2 =
    [ vector [| 0.; -1. |] // N
      vector [| 1.; -1. |] // NE
      vector [| 1.; 0. |] // E
      vector [| 1.; 1. |] // SE
      vector [| 0.; 1. |] // S
      vector [| -1.; 1. |] // SW
      vector [| -1.; 0. |] // W
      vector [| -1.; -1. |] ] // NW

let charAt m v =
    let row, col = mapToTuple int v
    let height, width = Array2D.length1 m, Array2D.length2 m

    if row < 0
       || row >= height
       || col < 0
       || col >= width then
        '.'
    else
        m[row, col]


let countAllFrom m (word: string) pos =
    directions2
    |> List.map (fun (dir) ->
        [ for i in 0 .. word.Length - 1 do
              let pos' = pos + (dir * (float i))
              charAt m pos' ]
        |> String.Concat)
    |> List.filter (fun s -> s = word)
    |> List.length

let countXmas m =
    m
    |> Array2D.mapi (fun row col _ -> countAllFrom m "XMAS" (vector [| row; col |]))
    |> Array2D.toSeq
    |> Seq.sum

let hasWordInXShapeFrom m pos =
    let word = "MAS"

    let directions =
        [ vector [| 1.; -1. |] // NE
          vector [| 1.; 1. |] // SE
          vector [| -1.; 1. |] // SW
          vector [| -1.; -1. |] ] // NW

    directions
    |> Seq.map (fun dir ->
        let start = pos + (dir * -1.)

        [ for i in 0 .. word.Length - 1 do
              charAt m (start + (dir * (float) i)) ]
        |> String.Concat)
    |> Seq.filter (fun w -> w = word)
    |> Seq.length
    >= 2


let countMasInXShape m =
    m
    |> Array2D.mapi (fun row col _ -> hasWordInXShapeFrom m (vector [| row; col |]))
    |> Array2D.toSeq
    |> Seq.filter id
    |> Seq.length

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let m = parse input
            let ans = countXmas m
            ans |> box


        member this.PartTwo(input: string) : obj =
            let m = parse input
            let ans = countMasInXShape m
            ans
