module Year2024Day15

open Common
open Extensions
open System

// Part one

let parse (input: string) =
    let parts = input.Split("\n\n")
    let m = parseToArray2D parts[0]
    let movements = parts[ 1 ].Split('\n') |> Seq.concat
    m, movements

let charToDir =
    function
    | '^' -> (0, -1)
    | 'v' -> (0, 1)
    | '<' -> (-1, 0)
    | '>' -> (1, 0)
    | _ -> failwith "invalid input"

let advance m robot c =
    let mutable m' = Array2D.copy m
    let dir = charToDir c

    let rec move from dir =
        let toX, toY = addTpl from dir

        match m'[toY, toX] with
        | '#' -> from
        | 'O' ->
            match move (toX, toY) dir with
            | samePos when samePos = (toX, toY) -> from
            | (newX, newY) ->
                m'[newY, newX] <- 'O'
                toX, toY
        | _ -> toX, toY

    let robot' = move robot dir

    if robot' = robot then
        m, robot
    else
        m'[snd robot, fst robot] <- '.'
        m'[snd robot', fst robot'] <- '@'
        m', robot'

// Part Two

type WideBox = { XMin: int; XMax: int; Y: int }

type WideState =
    { Robot: (int * int)
      Walls: list<int * int>
      Boxes: array<WideBox> }

let parse2 (input: string) =
    let parts = input.Split("\n\n")

    let lines =
        parts[ 0 ].Split('\n')
        |> Array.map (fun line ->
            line
            |> Seq.collect (function
                | '@' -> "@."
                | 'O' -> "[]"
                | c -> $"{c}{c}")
            |> Seq.toArray)


    let m = Array2D.init lines.Length lines[0].Length (fun y x -> lines[y][x])

    let robot =
        m
        |> Array2D.mapi (fun y x c -> (y, x, c))
        |> Array2D.toSeq
        |> Seq.find (fun (_, _, c) -> c = '@')
        |> (fun (y, x, _) -> x, y)

    let boxes =
        m
        |> Array2D.mapi (fun y x c -> (y, x, c))
        |> Array2D.toSeq
        |> Seq.filter (fun (_, _, c) -> c = '[')
        |> Seq.map (fun (y, x, _) -> { XMin = x; XMax = x + 1; Y = y })
        |> Seq.toArray

    let walls =
        m
        |> Array2D.mapi (fun y x c -> (y, x, c))
        |> Array2D.toSeq
        |> Seq.filter (fun (_, _, c) -> c = '#')
        |> Seq.map (fun (y, x, _) -> x, y)
        |> Seq.toList

    let state =
        { Robot = robot
          Boxes = boxes
          Walls = walls }


    let movements = parts[ 1 ].Split('\n') |> Seq.concat
    state, movements

let isPointInBox (x, y) box =
    box.Y = y && (box.XMin = x || box.XMax = x)

let advance2 state dir =
    let rec getBoxesToMove acc pos =
        let next = addTpl pos dir

        let collidesWithWall = state.Walls |> List.contains next

        let collidingBox =
            if collidesWithWall then
                None
            else
                state.Boxes
                |> Array.tryFindIndex (isPointInBox next)

        match collidingBox with
        | _ when collidesWithWall -> None
        | None -> Some(List.distinct acc)
        | Some idx ->
            let collidingBox = state.Boxes[idx]
            let acc' = idx :: acc

            let leftSide = collidingBox.XMin, collidingBox.Y
            let rightSide = collidingBox.XMax, collidingBox.Y
            let moveL = lazy getBoxesToMove acc' leftSide
            let moveR = lazy getBoxesToMove acc' rightSide

            match dir with
            | (-1, 0) -> moveL.Value
            | (1, 0) -> moveR.Value
            | _ when moveL.Value = None -> None
            | _ when moveR.Value = None -> None
            | _ ->
                let l = Option.get moveL.Value
                let r = Option.get moveR.Value
                let boxesToMove = List.append l r |> List.distinct
                Some boxesToMove

    let boxesToMove = getBoxesToMove [] state.Robot

    let moveBox box =
        let (dx, dy) = dir

        { box with
            XMin = box.XMin + dx
            XMax = box.XMax + dx
            Y = box.Y + dy }

    match boxesToMove with
    | None -> state
    | Some ids ->
        let newRobot = addTpl state.Robot dir

        let newBoxes =
            ids
            |> List.fold
                (fun (acc: array<WideBox>) idx ->
                    let movedBox = moveBox state.Boxes[idx]
                    acc[idx] <- movedBox
                    acc)
                (Array.copy state.Boxes)


        { state with
            Robot = newRobot
            Boxes = newBoxes }


let printState state =
    let w = state.Walls |> List.map fst |> List.max
    let h = state.Walls |> List.map snd |> List.max

    let lSideBoxes =
        state.Boxes
        |> Array.map (fun box -> box.XMin, box.Y)

    let rSideBoxes =
        state.Boxes
        |> Array.map (fun box -> box.XMax, box.Y)

    let m =
        Array2D.init h w (fun y x ->
            if state.Walls |> List.contains (x, y) then
                '#'
            elif state.Robot = (x, y) then
                '@'
            elif lSideBoxes |> Array.contains (x, y) then
                '['
            elif rSideBoxes |> Array.contains (x, y) then
                ']'
            else
                '.')

    Console.WriteLine(array2DToString m)


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let m, movements = parse input

            let robot =
                m
                |> Array2D.mapi (fun y x c -> (y, x, c))
                |> Array2D.toSeq
                |> Seq.find (fun (_, _, c) -> c = '@')
                |> (fun (y, x, _) -> x, y)

            let ans =
                movements
                |> Seq.fold
                    (fun (m, robot) c ->
                        let m', robot' = advance m robot c
                        //Console.WriteLine(array2DToString m')
                        m', robot')
                    (m, robot)
                |> fst
                |> Array2D.mapi (fun y x c -> (y, x, c))
                |> Array2D.toSeq
                |> Seq.filter (fun (_, _, c) -> c = 'O')
                |> Seq.sumBy (fun (y, x, _) -> y * 100 + x)



            ans |> box

        member this.PartTwo(input: string) : obj =
            let state, movements = parse2 input

            let finalState =
                movements
                |> Seq.fold
                    (fun state c ->
                        let dir = charToDir c
                        let state' = advance2 state dir
                        //printState state'
                        state')
                    (state)

            let ans =
                finalState.Boxes
                |> Seq.sumBy (fun box -> 100 * box.Y + box.XMin)

            ans |> box
