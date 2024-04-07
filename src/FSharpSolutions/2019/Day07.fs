namespace FSharpSolutions.Y2019.D07

open FSharpSolutions.Y2019

type Solver() =
    let setupAmplifiers opcodes phaseSettings =
        phaseSettings
        |> List.map (fun setting ->
            match IntCode.execute opcodes with
            | Halted _ -> failwith "Unexpected halt"
            | NeedsInput (_, nextComputeFn) ->
                match nextComputeFn setting with
                | Halted _ -> failwith "Unexpected halt"
                | NeedsInput (_, nextComputeFn') -> nextComputeFn')

    let doAmplifierPass ampStates prevOutput =
        let folder ((xs: (int -> Output) list), prevOutput: int) (computeFn: int -> Output) =
            match computeFn prevOutput with
            | Halted outputs -> xs, List.last outputs
            | NeedsInput (outputs, nextComputeFn) -> xs @ [ nextComputeFn ], List.last outputs

        ampStates |> List.fold folder ([], prevOutput)

    let calculateSignalNoLoop (opcodes: int []) (phaseSettings: int list) =
        // apply phase setting to all amplifiers to get initial state
        let initialAmpState = setupAmplifiers opcodes phaseSettings

        let _, output = doAmplifierPass initialAmpState 0
        output


    let calculateSignalWithLoop (opcodes: int []) (phaseSettings: int list) =
        // apply phase setting to all amplifiers to get initial state
        let initialAmpState = setupAmplifiers opcodes phaseSettings

        let rec feedbackLoop (ampStates: (int -> Output) list) (prevOutput: int) =
            match ampStates with
            // End condition, all amplifier programs have halted
            | [] -> prevOutput
            // Keep passing through amps until they have halted
            | _ ->
                let nextAmpStates, output = doAmplifierPass ampStates prevOutput

                feedbackLoop nextAmpStates output

        feedbackLoop initialAmpState 0

    let rec getAllPermutations xs =
        match xs with
        | [] -> [ [] ]
        | _ ->
            xs
            |> List.collect (fun x ->
                let others = xs |> List.filter (fun x' -> x' <> x)

                getAllPermutations others
                |> List.map (fun others' -> x :: others'))

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int

            let maxThrust =
                [ 0; 1; 2; 3; 4 ]
                |> getAllPermutations
                |> List.map (calculateSignalNoLoop opcodes)
                |> List.max

            maxThrust |> box

        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int

            let maxThrust =
                [ 5; 6; 7; 8; 9 ]
                |> getAllPermutations
                |> List.map (calculateSignalWithLoop opcodes)
                |> List.max

            maxThrust |> box
