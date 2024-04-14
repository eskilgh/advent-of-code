module Common

open System.Text

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
