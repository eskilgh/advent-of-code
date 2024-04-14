module Year2019Day08

open System

type Solver() =
    let (|Black|White|Transparent|) (value: int) =
        match value with
        | 0 -> Black
        | 1 -> White
        | 2 -> Transparent
        | _ -> failwith "Invalid pixel value"

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let digits = input |> Seq.map (fun c -> c - '0' |> int)
            let width, height = 25, 6

            let countOccurenses d ns =
                ns |> Seq.filter (fun n -> n = d) |> Seq.length

            digits
            |> Seq.chunkBySize (width * height)
            |> Seq.minBy (fun ds -> ds |> countOccurenses 0)
            |> (fun layer ->
                (layer |> countOccurenses 1)
                * (layer |> countOccurenses 2))
            |> box

        member this.PartTwo(input: string) : obj =
            let digits = input |> Seq.map (fun c -> c - '0' |> int)
            let width, height = 25, 6

            let pickFirstVisible =
                Seq.pick (fun pixel ->
                    match pixel with
                    | Black -> Some ' '
                    | White -> Some '█'
                    | Transparent -> None)

            digits
            |> Seq.chunkBySize (width * height)
            |> Seq.transpose
            |> Seq.map pickFirstVisible
            |> Seq.chunkBySize width
            |> Seq.map (fun row -> row |> Array.ofSeq |> String)
            |> String.concat "\n"
            |> box
