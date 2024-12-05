module Year2024Day03

open System.Text.RegularExpressions

type Instruction =
    | Multiply of int * int
    | Enable
    | Disable

let multiplyPattern = @"mul\(\d{1,3},\d{1,3}\)"
let doPattern = @"do\(\)"
let dontPattern = @"don't\(\)"

let toInstruction (s: string) =
    match s with
    | instruction when instruction.StartsWith("mul(") ->
        let parts = instruction[ "mul(".Length .. ].TrimEnd(')')
        let l = int (parts.Split(',')[0])
        let r = int (parts.Split(',')[1])
        Multiply(l, r)
    | "do()" -> Enable
    | "don't()" -> Disable
    | _ -> failwith "invalid input"


let parse pattern (input: string) =
    Regex(pattern).Matches(input)
    |> Seq.cast<Match>
    |> Seq.map (fun m -> toInstruction m.Value)

let applyInstructions =
    Seq.fold
        (fun (enabled, sum) instruction ->
            match instruction, enabled with
            | Multiply (a, b), true -> enabled, sum + a * b
            | Multiply _, false -> false, sum
            | Enable, _ -> true, sum
            | Disable, _ -> false, sum)
        (true, 0)
    >> snd


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            parse multiplyPattern input
            |> applyInstructions
            |> box


        member this.PartTwo(input: string) : obj =
            let pattern =
                [ multiplyPattern
                  doPattern
                  dontPattern ]
                |> String.concat "|"

            parse pattern input |> applyInstructions |> box
