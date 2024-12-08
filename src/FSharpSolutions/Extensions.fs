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

[<Extension>]
type List =
    [<Extension>]
    static member allUnorderedPairs(xs) =
        [ for i in 0 .. List.length xs - 1 do
              for j in (i + 1) .. List.length xs - 1 do
                  yield (xs.[i], xs.[j]) ]
