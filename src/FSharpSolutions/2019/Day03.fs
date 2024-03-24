namespace FSharpSolutions.Y2019.D03

type Point = { X: int; Y: int; Steps: int }

type Line =
    | Horizontal of Horizontal
    | Vertical of Vertical

and Horizontal =
    { Y: int
      StartX: int
      EndX: int
      Steps: int }

and Vertical =
    { X: int
      StartY: int
      EndY: int
      Steps: int }


type Solver() =
    let parseInstructions (input: string) =
        input.Split('\n')
        |> Array.toList
        |> (List.map (fun line ->
            line.Split(',')
            |> Array.toList
            |> List.map (fun (instruction) ->
                let direction = instruction.[0]
                let distance = int (instruction.[1..])
                (direction, distance))))

    let getPoints (instructions: (char * int) list) =
        let rec getPointsRec (position: Point) (instructions: (char * int) list) =
            match instructions with
            | [] -> []
            | (direction, distance) :: tl ->
                let nextPosition =
                    match direction with
                    | 'U' ->
                        { position with
                            Y = position.Y + distance
                            Steps = position.Steps + distance }
                    | 'D' ->
                        { position with
                            Y = position.Y - distance
                            Steps = position.Steps + distance }
                    | 'L' ->
                        { position with
                            X = position.X - distance
                            Steps = position.Steps + distance }
                    | 'R' ->
                        { position with
                            X = position.X + distance
                            Steps = position.Steps + distance }
                    | c -> failwith $"Unexpected direction {c}"

                nextPosition :: (getPointsRec nextPosition tl)

        getPointsRec { X = 0; Y = 0; Steps = 0 } instructions

    let toLine (p1: Point, p2: Point) =
        let { X = x1; Y = y1 } = p1
        let { X = x2; Y = y2 } = p2

        match (x1, y1, x2, y2) with
        | _ when x1 <> x2 ->
            Horizontal
                { StartX = x1
                  EndX = x2
                  Y = y1
                  Steps = p1.Steps }
        | _ when y1 <> y2 ->
            Vertical
                { StartY = y1
                  EndY = y2
                  X = x1
                  Steps = p1.Steps }
        | _ -> failwith "invalid line"

    let getIntersection (h: Horizontal) (v: Vertical) =
        let intersects =
            h.Y >= (min v.StartY v.EndY)
            && h.Y <= (max v.StartY v.EndY)
            && v.X >= (min h.StartX h.EndX)
            && v.X <= (max h.StartX h.EndX)

        if intersects then
            let sumSteps =
                h.Steps
                + abs (v.X - h.StartX)
                + v.Steps
                + abs (h.Y - v.StartY)

            Some { X = v.X; Y = h.Y; Steps = sumSteps }
        else
            None

    let findIntersection (l1: Line) (l2: Line) =
        match l1, l2 with
        | Horizontal l1, Vertical l2 -> getIntersection l1 l2
        | Vertical l1, Horizontal l2 -> getIntersection l2 l1
        | _ -> None

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let wires =
                parseInstructions input
                |> List.map (getPoints >> List.pairwise >> List.map toLine)

            let intersections =
                wires.Head
                |> Seq.collect (fun l1 ->
                    List.last wires
                    |> Seq.choose (fun l2 -> findIntersection l1 l2))

            intersections
            |> Seq.map (fun intersection -> abs intersection.X + abs intersection.Y)
            |> Seq.min
            |> box

        member this.PartTwo(input: string) : obj =
            let wires =
                parseInstructions input
                |> List.map (getPoints >> List.pairwise >> List.map toLine)

            let intersections =
                wires.Head
                |> Seq.collect (fun l1 ->
                    List.last wires
                    |> Seq.choose (fun l2 -> findIntersection l1 l2))

            intersections
            |> Seq.map (fun intersection -> intersection.Steps)
            |> Seq.min
            |> box
