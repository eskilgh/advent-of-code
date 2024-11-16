module Year2018Day01

let infiniteIter ns =
    let rec inner ns' =
        seq {
            match ns' with
            | [] -> yield! inner ns
            | n' :: rest ->
                yield n'
                yield! inner rest
        }

    inner ns

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            input.Split('\n')
            |> Array.map int
            |> Array.sum
            |> box

        member this.PartTwo(input: string) : obj =
            input.Split('\n')
            |> Array.map int
            |> Array.toList
            |> infiniteIter
            |> Seq.scan
                (fun (seen: Set<int>, sum, finished) n ->
                    let newSum = sum + n

                    if finished || seen.Contains(newSum) then
                        seen, sum, true
                    else
                        seen.Add(newSum), newSum, false)
                (Set.empty, 0, false)
            |> Seq.find (fun (_, _, finished) -> finished)
            |> (fun (_, repeated, _) -> repeated)
            |> box
