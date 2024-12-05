module Year2024Day02


/// Goes through the provided xs in pairwise fashion from left to right.
/// Performs the check predicate on every pair.
/// On check = true: proceeds with the next pair.
/// On check = false:
///   if tolerance <= 0 -> return false
///   else -> try to remove either of the two pairs and proceed to check the rest of the elements.
let pairwiseForAllWithTolerance check (tolerance: int) xs =
    let rec inner tolerance passed xs =
        match passed, xs with
        | _ when tolerance < 0 -> false
        | _, [] -> tolerance >= 0
        | [], x :: xs' -> inner tolerance [ x ] xs'
        | p :: _, x :: _ when check p x && tolerance = 0 -> false
        | p :: _, x :: xs' when check p x -> inner tolerance (x :: passed) xs'
        | p :: [], x :: xs' when not (check p x) ->
            let dropP = lazy inner (tolerance - 1) [ x ] xs'
            let dropX = lazy inner (tolerance - 1) passed xs'
            dropX.Value || dropP.Value
        | p :: p' :: passed', x' :: xs' when not (check p x') ->
            let dropP =
                lazy
                    (check p' x'
                     && inner (tolerance - 1) (x' :: p' :: passed') xs')

            let dropX = lazy inner (tolerance - 1) passed xs'
            dropX.Value || dropP.Value

    inner tolerance [] xs


let isSafeReport tolerance (ns: list<int>) =
    let check sign a b =
        let diff = (b - a) * sign
        diff >= 1 && diff <= 3

    let asc = check 1
    let desc = check -1

    ns |> pairwiseForAllWithTolerance asc tolerance
    || ns |> pairwiseForAllWithTolerance desc tolerance

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let reports =
                input.Split('\n')
                |> Array.map (fun line -> line.Split(' ') |> List.ofArray |> List.map int)

            let tolerance = 0

            reports
            |> Array.filter (isSafeReport tolerance)
            |> Array.length
            |> box


        member this.PartTwo(input: string) : obj =
            let reports =
                input.Split('\n')
                |> Array.map (fun line -> line.Split(' ') |> List.ofArray |> List.map int)

            let tolerance = 1

            reports
            |> Array.filter (isSafeReport tolerance)
            |> Array.length
            |> box
