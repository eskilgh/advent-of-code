module Year2018Day02

open System

let countTwosAndThrees charCounts =
    charCounts
    |> Seq.fold
        (fun (twos, threes) (charCount: Map<char, int>) ->
            let incTwo =
                if charCount.Values.Contains(2) then
                    1
                else
                    0

            let incThree =
                if charCount.Values.Contains(3) then
                    1
                else
                    0

            (twos + incTwo, threes + incThree))
        (0, 0)

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            input.Split('\n')
            |> Seq.map (fun boxId -> boxId |> Seq.countBy id |> Map.ofSeq)
            |> countTwosAndThrees
            |> (fun (twos, threes) -> twos * threes)
            |> box

        member this.PartTwo(input: string) : obj =
            let ids = input.Split('\n')
            let desiredLength = (ids |> Seq.head |> Seq.length) - 1

            Seq.allPairs ids ids
            |> Seq.pick (fun (id1, id2) ->
                let common =
                    Seq.zip id1 id2
                    |> Seq.where (fun (c1, c2) -> c1 = c2)
                    |> Seq.map (fun (c, _) -> c)
                    |> String.Concat

                if common.Length = desiredLength then
                    Some common
                else
                    None)
