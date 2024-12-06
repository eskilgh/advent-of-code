module Extensions

open System.Runtime.CompilerServices

[<Extension>]
type Array2D =
    [<Extension>]
    static member toSeq(array2d: 'T [,]) : seq<'T> =
        seq {
            for i in 0 .. Array2D.length1 array2d - 1 do
                for j in 0 .. Array2D.length2 array2d - 1 do
                    yield array2d.[i, j]
        }
