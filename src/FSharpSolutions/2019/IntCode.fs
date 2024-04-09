namespace FSharpSolutions.Y2019

type Output =
    | NeedsInput of int64 list * (int64 -> Output)
    | Halted of int64 list

type Memory =
    private
        { map: Map<int64, int64> }

    static member ofArray(values: int64 array) =
        let map =
            values
            |> Array.mapi (fun index value -> int64 index, value)
            |> Map.ofArray

        { map = map }

    member m.Get(index) =
        match m.map.TryFind index with
        | Some value -> value
        | None -> 0

    member m.Item
        with get (index: int64) = m.Get index

    member m.Add (index) value = { map = m.map |> Map.add index value }



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

    let (|Relative|_|) value =
        if value = 2 then
            Some Relative
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

    let (|RelativeBaseOffset|_|) value =
        if value = 9 then
            Some RelativeBaseOffset
        else
            None

    let (|Halt|_|) value = if value = 99 then Some Halt else None

    type InstructionType =
        | Read
        | Write

    let execute (opcodes: int64 []) =
        let memory = opcodes |> Memory.ofArray

        let rec executeRec (memory: Memory) (i: int64) (rBase: int64) (outputs: int64 list) : Output =
            let getParam paramType offset =
                let mode =
                    (memory.[i]) / (pown 10L (offset + 1)) % 10L
                    |> int

                match mode, paramType with
                | Position, Write -> memory.[i + (int64 offset)]
                | Position, Read -> memory.[memory.[i + (int64 offset)]]
                | Immediate, Write -> failwith "Write instruction params should never be in immediate mode"
                | Immediate, Read -> memory.[i + (int64 offset)]
                | Relative, Write -> rBase + memory.[i + (int64 offset)]
                | Relative, Read -> memory.[rBase + memory.[i + (int64 offset)]]
                | _ -> failwith $"Unexpected parameter mode {mode}"

            let getAddress = getParam Write
            let getValue = getParam Read

            let opcode = (memory.[i]) % 100L |> int

            match opcode with
            | Add op
            | Multiply op ->
                let a, b, address = (getValue 1, getValue 2, getAddress 3)
                let newMem = memory.Add address (op a b)
                executeRec newMem (i + 4L) rBase outputs
            | Input ->

                let address = getAddress 1

                let callback inp =
                    let newMem = memory.Add address inp
                    executeRec newMem (i + 2L) rBase []

                NeedsInput(outputs, callback)

            | Output ->
                let a = getValue 1
                executeRec memory (i + 2L) rBase (outputs @ [ a ])
            | JumpIfTrue
            | JumpIfFalse ->
                let a, b = getValue 1, getValue 2

                let newIndex =
                    match opcode with
                    | 5 when a <> 0 -> b
                    | 6 when a = 0 -> b
                    | _ -> (i + 3L)

                executeRec memory newIndex rBase outputs
            | LessThan comparator
            | Equals comparator ->
                let a, b, address = getValue 1, getValue 2, getAddress 3
                let value = if comparator a b then 1L else 0L
                let newMem = memory.Add address value
                executeRec newMem (i + 4L) rBase outputs
            | RelativeBaseOffset ->
                let a = getValue 1
                executeRec memory (i + 2L) (rBase + a) outputs
            | Halt -> Halted outputs
            | n -> failwith $"Invalid opcode: {n}"

        executeRec memory 0 0 []
