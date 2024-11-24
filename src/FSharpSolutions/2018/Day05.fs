module Year2018Day05

open System

let hasOppositePolarity (a: char) (b: char) = ((int a) - (int b) |> abs) = 32

let react polymer =
    polymer
    |> Seq.fold
        (fun keepers curr ->
            match keepers with
            | hd :: tl when hasOppositePolarity hd curr -> tl
            | keepers -> curr :: keepers)
        []
    |> List.rev
    |> String.Concat

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj = input |> react |> Seq.length |> box

        member this.PartTwo(input: string) : obj =
            let units = input.ToLower() |> Seq.distinct

            units
            |> Seq.map (fun unit ->
                input
                |> Seq.filter (fun c -> Char.ToLower(c) <> unit)
                |> String.Concat
                |> react
                |> Seq.length)
            |> Seq.min
            |> box
