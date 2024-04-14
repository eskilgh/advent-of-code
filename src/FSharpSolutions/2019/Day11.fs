module Year2019Day11

open IntCode
open Common
open System

type Solver() =
    let (|Black|_|) value = if value = 0L then Some Black else None
    let (|White|_|) value = if value = 1L then Some White else None
    let (|Left|_|) value = if value = 0L then Some Left else None
    let (|Right|_|) value = if value = 1L then Some Right else None

    let turn (v_x, v_y) direction =
        match direction with
        | Left -> -v_y, v_x
        | Right -> v_y, -v_x
        | _ -> failwith "invalid direction"

    let runProgram opcodes startColor =
        let rec runRec computeFn position direction panel =
            let currentcolor =
                match panel |> Map.tryFind position with
                | None -> 0L
                | Some value -> value

            match computeFn currentcolor with
            | Halted _ -> panel
            | NeedsInput (outputs, newComputeFn) ->
                let newDirection = turn direction outputs[1]
                let newPanel = panel |> Map.add position outputs.[0]
                let newPosition = fst position + fst newDirection, snd position + snd newDirection
                runRec newComputeFn newPosition newDirection newPanel

        let startPosition = 0, 0
        let startDirection = 0, -1
        let startPanel = Map.ofList [ startPosition, startColor ]

        let startComputeFn =
            match IntCode.execute opcodes with
            | Halted _ -> failwith "unexpected halt"
            | NeedsInput (_, computeFn) -> computeFn

        runRec startComputeFn startPosition startDirection startPanel

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64
            runProgram opcodes 0 |> Map.count |> box

        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64
            let panels = runProgram opcodes 1

            let xMin, xMax, yMin, yMax =
                panels
                |> Map.toSeq
                |> Seq.map (fun (point, _) -> point)
                |> Seq.fold
                    (fun (xMin, xMax, yMin, yMax) (x, y) -> min xMin x, max xMax x, min yMin y, max yMax y)
                    (Int32.MaxValue, Int32.MinValue, Int32.MaxValue, Int32.MinValue)

            let width = xMax - xMin + 1
            let height = yMax - yMin + 1
            let whitePanels = panels |> Map.filter (fun _ color -> color = 1L)

            let arr =
                Array2D.init height width (fun y x ->
                    if whitePanels.ContainsKey(xMax - x, yMin + y) then
                        '█'
                    else
                        ' ')

            array2DToString arr
