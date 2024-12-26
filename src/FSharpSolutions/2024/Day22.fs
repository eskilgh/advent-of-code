module Year2024Day22

let mix a b = a ^^^ b

let prune n = n % 16777216L

let nextSecret n =
    n
    |> fun n' -> mix n' (n' <<< 6)
    |> fun n' -> prune n'
    |> fun n' -> mix n' (n' >>> 5)
    |> fun n' -> prune n'
    |> fun n' -> mix n' (n' <<< 11)
    |> fun n' -> prune n'

let generateSecrets iters seed =
    { 1..iters }
    |> Seq.scan (fun prev _ -> nextSecret prev) seed


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let initialSecrets = input.Split('\n') |> Array.map int64

            initialSecrets
            |> Seq.map (generateSecrets 2000)
            |> Seq.sumBy Seq.last
            |> box

        member this.PartTwo(input: string) : obj =
            let initialSecrets = input.Split('\n') |> Array.map int64

            initialSecrets
            |> Seq.collect (fun seed ->
                seed
                |> generateSecrets 2000
                |> Seq.map (fun secret -> secret % 10L)
                |> Seq.pairwise
                |> Seq.map (fun (prevPrice, price) ->
                    let diff = price - prevPrice
                    price, diff)
                |> Seq.windowed 4
                |> Seq.map (fun window ->
                    let diffs =
                        window
                        |> Array.map snd
                        |> (fun ds -> (ds[0], ds[1], ds[2], ds[3]))

                    let price = window |> Array.last |> fst
                    diffs, price)
                |> Seq.distinctBy fst
                |> Seq.toArray)
            |> Seq.groupBy fst
            |> Seq.map (snd >> Seq.sumBy snd)
            |> Seq.max
            |> box
