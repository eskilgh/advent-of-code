module Year2019Day07

open IntCode

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
        let folder ((xs: (int64 -> Output) list), prevOutput: int64) (computeFn: int64 -> Output) =
            match computeFn prevOutput with
            | Halted outputs -> xs, List.last outputs
            | NeedsInput (outputs, nextComputeFn) -> xs @ [ nextComputeFn ], List.last outputs

        ampStates |> List.fold folder ([], prevOutput)

    let calculateSignalNoLoop (opcodes: int64 []) (phaseSettings: int64 list) =
        // apply phase setting to all amplifiers to get initial state
        let initialAmpState = setupAmplifiers opcodes phaseSettings

        let _, output = doAmplifierPass initialAmpState 0
        output


    let calculateSignalWithLoop (opcodes: int64 []) (phaseSettings: int64 list) =
        // apply phase setting to all amplifiers to get initial state
        let initialAmpState = setupAmplifiers opcodes phaseSettings

        let rec feedbackLoop (ampStates: (int64 -> Output) list) (prevOutput: int64) =
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
            let opcodes = input.Split(',') |> Array.map int64

            let maxThrust =
                [ 0L; 1L; 2L; 3L; 4L ]
                |> getAllPermutations
                |> List.map (calculateSignalNoLoop opcodes)
                |> List.max

            maxThrust |> box

        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64

            let maxThrust =
                [ 5L; 6L; 7L; 8L; 9L ]
                |> getAllPermutations
                |> List.map (calculateSignalWithLoop opcodes)
                |> List.max

            maxThrust |> box
