module Year2024Day15

open Common
open Extensions
open System

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

        member this.PartTwo(input: string) : obj = "Not available"
