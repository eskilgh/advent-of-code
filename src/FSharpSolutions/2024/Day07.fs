module Year2024Day07


let parse (input: string) =
    input.Split('\n')
    |> Array.map (fun line ->
        let parts = line.Split(": ")
        let target = int64 parts[0]

        let nums =
            parts[ 1 ].Split(' ')
            |> List.ofArray
            |> List.map (int64)

        target, nums)

#nowarn 86
let (||) a b = int64 ((string a) + (string b))

let canMakeTarget target (ops: list<(int64 -> int64 -> int64)>) (nums: list<int64>) =
    let rec inner sum nums =
        match nums with
        | _ when sum > target -> false
        | [] -> sum = target
        | n :: nums' ->
            ops
            |> List.exists (fun op -> inner (op sum n) nums')


    inner (List.head nums) (List.tail nums)

let sumOfValidEquations ops =
    Seq.filter (fun (target, nums) -> canMakeTarget target ops nums)
    >> Seq.sumBy (fun (target, _) -> target)


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let equations = parse input
            let ops = [ (+); (*) ]

            sumOfValidEquations ops equations |> box


        member this.PartTwo(input: string) : obj =
            let equations = parse input
            let ops = [ (+); (*); (||) ]

            sumOfValidEquations ops equations |> box
