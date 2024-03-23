namespace FSharpSolutions.Y2019.D02
 
type Solver() =
    let rec program (opcodes : int[]) i = 
        match opcodes.[i] with
        | 1 | 2 -> 
            let op = if opcodes.[i] = 1 then (+) else (*)
            let a, b, c =
                match opcodes.[i+1 .. i+3] with
                | [| a; b; c; |] -> (a, b, c)
                | _ -> failwith "Expected exactly three elements"
            opcodes.[c] <- op opcodes.[a] opcodes.[b]
            program opcodes (i + 4)
        | 99 -> opcodes.[0]
        | _ -> -1

    interface Shared.ISolver with

        member this.PartOne(input: string): obj =
            let opcodes = input.Split(',') |> Array.map int
            opcodes.[1] <- 12
            opcodes.[2] <- 2
            program opcodes 0 |> box

        member this.PartTwo(input: string): obj =
            let opcodes = input.Split(',') |> Array.map int
            let p (noun, verb) = 
                let opcodes' = Array.copy opcodes
                opcodes'.[1] <- noun
                opcodes'.[2] <- verb
                let output = program opcodes' 0
                output = 19690720
                    
            let (noun, verb) = 
                seq {
                    for noun in 1 .. 99 do
                        for verb in 1 .. 99 do
                            (noun, verb)
                } |> Seq.find p
            100 * noun + verb |> box


 