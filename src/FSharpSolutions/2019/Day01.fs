module Year2019Day01

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            input.Split('\n')
            |> Array.map int
            |> Array.map (fun m -> m / 3 - 2)
            |> Array.sum
            |> box

        member this.PartTwo(input: string) : obj =
            let rec calculateFuel m =
                match (m / 3 - 2) with
                | m' when m' <= 0 -> 0
                | m' -> (m' + calculateFuel m')

            input.Split('\n')
            |> Array.map int
            |> Array.map calculateFuel
            |> Array.sum
            |> box
