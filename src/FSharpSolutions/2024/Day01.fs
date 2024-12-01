module Year2024Day01

let parse (input: string) =
    input.Split('\n')
    |> Array.map (fun line ->
        let parts = line.Split("  ")
        int parts[0], int parts[1])
    |> Array.unzip

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let leftIds, rightIds = parse input

            leftIds
            |> Array.sort
            |> Array.zip (rightIds |> Array.sort)
            |> Array.map (fun (l, r) -> abs (l - r))
            |> Array.sum
            |> box

        member this.PartTwo(input: string) : obj =
            let leftIds, rightIds =
                input.Split('\n')
                |> Array.map (fun line ->
                    let parts = line.Split("  ")
                    int parts[0], int parts[1])
                |> Array.unzip

            let appearances = rightIds |> Seq.countBy id |> Map.ofSeq

            leftIds
            |> Seq.map (fun locationId ->
                match Map.tryFind locationId appearances with
                | Some count -> count * locationId
                | None -> 0)
            |> Seq.sum
            |> box
