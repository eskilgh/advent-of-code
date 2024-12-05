module Year2024Day05

let parse (input: string) =
    let parts = input.Split("\n\n")

    let rules =
        parts[ 0 ].Split('\n')
        |> Array.map (fun line ->
            let raw = line.Split('|')
            let preceder, succeeder = int raw[0], int raw[1]
            preceder, succeeder)
        |> Array.groupBy (fun (pre, _) -> pre)
        |> Array.map (fun (pre, vals) ->
            let succeeders = vals |> Array.map snd |> Set.ofArray
            (pre, succeeders))
        |> Map.ofArray

    let updates =
        parts[ 1 ].Split('\n')
        |> Array.map (fun line -> line.Split(',') |> Array.map int |> List.ofArray)

    rules, updates

let followsRules (rules: Map<int, Set<int>>) (seen: Set<int>) (n: int) =
    match rules.TryFind(n) with
    | Some mustPreceed -> (Set.intersect seen mustPreceed).Count = 0
    | None -> true

let areNumsInRightOrder (rules: Map<int, Set<int>>) (nums: list<int>) =
    let rec inner seen nums =
        match nums with
        | [] -> true
        | n :: tl -> followsRules rules seen n && inner (seen.Add n) tl

    inner Set.empty nums

let compare (rules: Map<int, Set<int>>) a b =
    let aMustPreceed = Option.defaultValue Set.empty (rules.TryFind a)
    let bMustPreceed = Option.defaultValue Set.empty (rules.TryFind b)

    if Set.contains b aMustPreceed then -1
    elif Set.contains a bMustPreceed then 1
    else 0

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let rules, updates = parse input

            updates
            |> Array.filter (areNumsInRightOrder rules)
            |> Array.map (fun nums -> nums[nums.Length / 2])
            |> Array.sum
            |> box

        member this.PartTwo(input: string) : obj =
            let rules, updates = parse input

            updates
            |> Array.filter (not << areNumsInRightOrder rules)
            |> Array.map (List.sortWith (compare rules))
            |> Array.map (fun nums -> nums[nums.Length / 2])
            |> Array.sum
            |> box
