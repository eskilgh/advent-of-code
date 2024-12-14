module Year2024Day14

open Common
open System
open System.Collections.Generic

type Robot =
    { Position: int * int
      Velocity: int * int }

let parse (input: string) =
    input.Split('\n')
    |> Array.map (fun line ->
        let parts = line.Split(' ')
        let posParts = (parts[ 0 ].Substring(2)).Split(',')
        let velParts = (parts[ 1 ].Substring(2)).Split(',')
        let pos = int posParts[0], int posParts[1]
        let vel = int velParts[0], int velParts[1]
        { Position = pos; Velocity = vel })

let addTpl (ax, ay) (bx, by) = ax + bx, ay + by

let multTpl (x, y) scalar = x * scalar, y * scalar

let negMod x m =
    let result = x % m

    if result < 0 then
        result + m
    else
        result

let fitToBoard width height (x, y) = negMod x width, negMod y height


let advance width height seconds robot =
    let next = addTpl robot.Position (multTpl robot.Velocity seconds)
    { robot with Position = fitToBoard width height next }

type Quadrant =
    | Q1
    | Q2
    | Q3
    | Q4

let getQuadrant width height robot =
    let midWith = width / 2
    let midHeight = height / 2

    match robot.Position with
    | x, _ when x = midWith -> None
    | _, y when y = midHeight -> None
    | x, y when x > midWith && y < midHeight -> Some Q1
    | x, y when x < midWith && y < midHeight -> Some Q2
    | x, y when x < midWith && y > midHeight -> Some Q3
    | x, y when x > midWith && y > midHeight -> Some Q4
    | _ -> failwith "Unexpected error"


let visualize width height robots =
    let m =
        Array2D.init height width (fun y x ->
            if (robots
                |> Array.exists (fun r -> r.Position = (x, y))) then
                '#'
            else
                '.')

    array2DToString m

let findChristmasTree width height robots =
    let containsTree robots =
        (visualize width height robots)
            .Contains("#########")

    let rec inner acc robots =
        if containsTree robots then
            acc
        else
            let robots' = robots |> Array.map (advance width height 1)
            inner (acc + 1) robots'

    inner 0 robots




type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let robots = parse input
            let width = 11
            let height = 7
            let seconds = 100

            let ans =
                robots
                |> Array.map (advance width height seconds)
                |> Array.choose (getQuadrant width height)
                |> Array.countBy id
                |> Array.fold (fun acc (_, count) -> acc * count) 1

            ans |> box

        member this.PartTwo(input: string) : obj =
            let robots = parse input
            let width = 101
            let height = 103
            let ans = findChristmasTree width height robots
            ans |> box
