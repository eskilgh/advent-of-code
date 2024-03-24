namespace FSharpSolutions.Y2019.D05

module IntCode =
    let execute (opcodes: int []) (input: int) =
        let rec executeRec i =
            seq {
                let getParam offset =
                    let mode = opcodes.[i] / (pown 10 (offset + 1)) % 10

                    match mode with
                    | 0 -> opcodes.[opcodes.[i + offset]]
                    | 1 -> opcodes.[i + offset]
                    | _ -> failwith "Unexpected parameter mode"

                let opcode = opcodes.[i] % 100

                match opcode with
                | 1
                | 2 ->
                    let op = if opcode = 1 then (+) else (*)
                    let a, b, address = (getParam 1, getParam 2, opcodes.[i + 3])
                    opcodes.[address] <- op a b
                    yield! executeRec (i + 4)
                | 3 ->
                    let address = opcodes[i + 1]
                    opcodes.[address] <- input
                    yield! executeRec (i + 2)
                | 4 ->
                    let a = getParam 1
                    yield a
                    yield! executeRec (i + 2)
                | 5
                | 6 ->
                    let a, b = getParam 1, getParam 2

                    match opcode with
                    | 5 when a <> 0 -> yield! executeRec b
                    | 6 when a = 0 -> yield! executeRec b
                    | _ -> yield! executeRec (i + 3)
                | 7
                | 8 ->
                    let a, b, address = getParam 1, getParam 2, opcodes[i + 3]
                    let comparator = if opcode = 7 then (<) else (=)
                    let value = if comparator a b then 1 else 0
                    opcodes.[address] <- value
                    yield! executeRec (i + 4)
                | 99 -> yield! []
                | n -> failwith $"Invalid opcode: {n}"
            }

        executeRec 0

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int
            let programInput = 1
            let outputs = IntCode.execute opcodes programInput

            outputs |> Seq.last |> box

        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int
            let programInput = 5
            let outputs = IntCode.execute opcodes programInput
            outputs |> Seq.last |> box
