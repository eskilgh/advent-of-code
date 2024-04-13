module Common

open System.Text

let array2DToString (array: 'a [,]) =
    let sb = new StringBuilder()

    for row in 0 .. Array2D.length1 array - 1 do
        for col in 0 .. Array2D.length2 array - 1 do
            sb.Append(array.[row, col]) |> ignore

        sb.Append('\n') |> ignore

    sb.ToString()
