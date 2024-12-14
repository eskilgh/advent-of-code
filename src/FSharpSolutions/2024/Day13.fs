module Year2024Day13

let parseCoords (line: string) =
    let ss = (line.Split(':')[1]).Split(',')
    let x = int64 ((ss[ 0 ].Split('X')[1]).Substring(1))
    let y = int64 ((ss[ 1 ].Split('Y')[1]).Substring(1))
    x, y

let parsePart (part: string) =
    let lines = part.Split('\n')
    let a = parseCoords lines[0]
    let b = parseCoords lines[1]
    let prize = parseCoords lines[2]
    a, b, prize


let parse (input: string) =
    input.Split("\n\n") |> Array.map parsePart

type Strategy = { APress: int64; BPress: int64 }

let findOptimumStrategy ((ax, ay), (bx, by), (tx, ty)) =
    let canGetThereWithOnlyBPresses (distX, distY) =
        distX % bx = 0L
        && distY % by = 0L
        && distX / bx = distY / by

    let rec inner acc (currX, currY) =
        let distToTarget = tx - currX, ty - currY

        match distToTarget with
        | distX, distY when distX < 0L || distY < 0L -> { APress = 0L; BPress = 0L }
        | distX, distY when canGetThereWithOnlyBPresses (distX, distY) -> { acc with BPress = int (distX / bx) }
        | _ ->
            let acc' = { acc with APress = acc.APress + 1L }
            let nextPos = (currX + ax, currY + ay)
            inner acc' nextPos

    inner { APress = 0L; BPress = 0L } (0L, 0L)

///
/// APress*ax + BPress*bx = tx
/// APress*ay + BPress*by = ty
///
/// Solve with Cramer's rule
let findOptimumStrategyCramer ((ax, ay), (bx, by), (tx, ty)) =
    let aPress = (tx * by - ty * bx) / (ax * by - ay * bx)
    let bPress = (ax * ty - ay * tx) / (ax * by - ay * bx)
    let dest = ax * aPress + bx * bPress, ay * aPress + by * bPress

    if (dest = (tx, ty)) then
        { APress = aPress; BPress = bPress }
    else
        { APress = 0L; BPress = 0L }

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let machines = parse input

            let ans =
                machines
                |> Seq.map findOptimumStrategy
                |> Seq.sumBy (fun strategy -> strategy.APress * 3L + strategy.BPress)

            ans



        member this.PartTwo(input: string) : obj =
            let machines = parse input

            let ans =
                machines
                |> Seq.map (fun ((ax, ay), (bx, by), (tx, ty)) ->
                    ((ax, ay), (bx, by), (tx + 10000000000000L, ty + 10000000000000L)))
                |> Seq.map findOptimumStrategyCramer
                |> Seq.sumBy (fun strategy -> strategy.APress * 3L + strategy.BPress)


            ans |> box
