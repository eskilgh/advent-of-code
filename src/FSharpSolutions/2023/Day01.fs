namespace FSharpSolutions.Y2023.D01

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string): obj = 
            "hello from f# part one"
        member this.PartTwo(input: string): obj = 
            "hello from f# part two"