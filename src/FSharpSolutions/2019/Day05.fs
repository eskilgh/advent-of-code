module Year2019Day05

module IntCode =
    let (|Position|_|) value =
        if value = 0 then
            Some Position
        else
            None

    let (|Immediate|_|) value =
        if value = 1 then
            Some Immediate
        else
            None

    let (|Add|_|) value =
        if value = 1 then
            Some(fun a b -> a + b)
        else
            None

    let (|Multiply|_|) value =
        if value = 2 then
            Some(fun a b -> a * b)
        else
            None

    let (|Input|_|) value = if value = 3 then Some Input else None
    let (|Output|_|) value = if value = 4 then Some Output else None

    let (|JumpIfTrue|_|) value =
        if value = 5 then
            Some JumpIfTrue
        else
            None

    let (|JumpIfFalse|_|) value =
        if value = 6 then
            Some JumpIfFalse
        else
            None

    let (|LessThan|_|) value =
        if value = 7 then
            Some(fun a b -> a < b)
        else
            None

    let (|Equals|_|) value =
        if value = 8 then
            Some(fun a b -> a = b)
        else
            None

    let (|Halt|_|) value = if value = 99 then Some Halt else None

    let execute (opcodes: int []) (input: int) =
        let rec executeRec i =
            seq {
                let getParam offset =
                    let mode = opcodes.[i] / (pown 10 (offset + 1)) % 10

                    match mode with
                    | Position -> opcodes.[opcodes.[i + offset]]
                    | Immediate -> opcodes.[i + offset]
                    | _ -> failwith "Unexpected parameter mode"

                let opcode = opcodes.[i] % 100

                match opcode with
                | Add op
                | Multiply op ->
                    let a, b, address = (getParam 1, getParam 2, opcodes.[i + 3])
                    opcodes.[address] <- op a b
                    yield! executeRec (i + 4)
                | Input ->
                    let address = opcodes[i + 1]
                    opcodes.[address] <- input
                    yield! executeRec (i + 2)
                | Output ->
                    let a = getParam 1
                    yield a
                    yield! executeRec (i + 2)
                | JumpIfTrue
                | JumpIfFalse ->
                    let a, b = getParam 1, getParam 2

                    match opcode with
                    | 5 when a <> 0 -> yield! executeRec b
                    | 6 when a = 0 -> yield! executeRec b
                    | _ -> yield! executeRec (i + 3)

                | LessThan comparator
                | Equals comparator ->
                    let a, b, address = getParam 1, getParam 2, opcodes[i + 3]
                    let value = if comparator a b then 1 else 0
                    opcodes.[address] <- value
                    yield! executeRec (i + 4)
                | Halt -> yield! []
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
