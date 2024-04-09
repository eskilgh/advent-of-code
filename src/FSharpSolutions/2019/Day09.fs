namespace FSharpSolutions.Y2019.D09

open FSharpSolutions.Y2019

type ProgramMode =
    | Test
    | Boost


type Solver() =

    let runProgram mode opcodes =
        let input =
            match mode with
            | Test -> 1
            | Boost -> 2

        match IntCode.execute opcodes with
        | Halted _ -> failwith "Unexpected halt"
        | NeedsInput (outputs', computeFn) ->
            match computeFn input with
            | Halted outputs'' -> outputs' @ outputs''
            | NeedsInput _ -> failwith "Unexpected non-halt"

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64

            let boostKeycode = runProgram Test opcodes |> List.exactlyOne
            boostKeycode


        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64

            let boostKeycode = runProgram Boost opcodes |> List.exactlyOne
            boostKeycode
