module Common

open System.Text
open FSharp.Stats

let parseToArray2D (input: string) =
    let lines = input.Split('\n')

    Array2D.init lines.Length lines[0].Length (fun y x -> lines[y][x])

let outOfBounds (array: 'a [,]) (row, col) =
    let h = Array2D.length1 array
    let w = Array2D.length2 array
    row < 0 || row >= h || col < 0 || col >= w

let rotate90 (v: Vector<float>) = vector [| v.[1]; -v.[0] |]

let mapToTuple f (v: Vector<float>) = f v.[0], f v.[1]

let array2DToString (array: 'a [,]) =
    let sb = new StringBuilder()

    for row in 0 .. Array2D.length1 array - 1 do
        for col in 0 .. Array2D.length2 array - 1 do
            sb.Append(array.[row, col]) |> ignore

        sb.Append('\n') |> ignore

    sb.ToString()

let rec gcd a b = if b = 0L then a else gcd b (a % b)

let lcm a b =
    if a = 0L || b = 0L then
        0L
    else
        abs (a * b) / gcd a b
