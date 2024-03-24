namespace FSharpSolutions.Y2019.D04

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            input.Split('-')
            |> Array.map int
            |> function
                | [| lower; upper |] -> seq { lower..upper }
                | _ -> failwith "invalid input"
            |> Seq.filter (fun candidate ->
                let digitsNeverDecrease =
                    candidate
                    |> string
                    |> Seq.pairwise
                    |> Seq.forall (fun (l, r) -> l <= r)

                let hasAdjacentDigits =
                    candidate
                    |> string
                    |> Seq.pairwise
                    |> Seq.exists (fun (l, r) -> l = r)

                digitsNeverDecrease && hasAdjacentDigits)
            |> Seq.length
            |> box



        member this.PartTwo(input: string) : obj =
            input.Split('-')
            |> Array.map int
            |> function
                | [| lower; upper |] -> seq { lower..upper }
                | _ -> failwith "invalid input"
            |> Seq.filter (fun candidate ->
                let digitsNeverDecrease =
                    candidate
                    |> string
                    |> Seq.pairwise
                    |> Seq.forall (fun (l, r) -> l <= r)

                let hasExactlyTwoAdjacentDigits =
                    candidate
                    |> string
                    |> Seq.fold
                        (fun (prev: char, counts: int list) current ->
                            let newCounts =
                                match counts with
                                | hd :: tl when prev = current -> hd + 1 :: tl
                                | _ -> 1 :: counts

                            (current, newCounts))
                        ('_', [])
                    |> snd
                    |> Seq.exists (fun count -> count = 2)

                digitsNeverDecrease && hasExactlyTwoAdjacentDigits)
            |> Seq.length
            |> box
