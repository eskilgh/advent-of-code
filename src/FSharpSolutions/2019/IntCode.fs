namespace FSharpSolutions.Y2019

type Output =
    | NeedsInput of int list * (int -> Output)
    | Halted of int list

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

    let (|JumpIfPredicate|_|) value =
        match value with
        | 5 -> Some(fun a -> a <> 0)
        | 6 -> Some(fun a -> a = 0)
        | _ -> None

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

    let execute (opcodes: int []) =
        let opcodesCopy = opcodes |> Array.copy

        let rec executeRec (i: int) (outputs: int list) : Output =
            let getParam offset =
                let mode = opcodesCopy.[i] / (pown 10 (offset + 1)) % 10

                match mode with
                | Position -> opcodesCopy.[opcodesCopy.[i + offset]]
                | Immediate -> opcodesCopy.[i + offset]
                | _ -> failwith "Unexpected parameter mode"

            let opcode = opcodesCopy.[i] % 100

            match opcode with
            | Add op
            | Multiply op ->
                let a, b, address = (getParam 1, getParam 2, opcodesCopy.[i + 3])
                opcodesCopy.[address] <- op a b
                executeRec (i + 4) outputs
            | Input ->
                let address = opcodesCopy[i + 1]

                let callback inp =
                    opcodesCopy.[address] <- inp
                    executeRec (i + 2) []

                NeedsInput(outputs, callback)

            | Output ->
                let a = getParam 1
                executeRec (i + 2) (outputs @ [ a ])
            | JumpIfTrue
            | JumpIfFalse ->
                let a, b = getParam 1, getParam 2

                let newIndex =
                    match opcode with
                    | 5 when a <> 0 -> b
                    | 6 when a = 0 -> b
                    | _ -> (i + 3)

                executeRec newIndex outputs
            | LessThan comparator
            | Equals comparator ->
                let a, b, address = getParam 1, getParam 2, opcodesCopy[i + 3]
                let value = if comparator a b then 1 else 0
                opcodesCopy.[address] <- value
                executeRec (i + 4) outputs
            | Halt -> Halted outputs
            | n -> failwith $"Invalid opcode: {n}"

        executeRec 0 []
