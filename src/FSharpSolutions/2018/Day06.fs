module Year2018Day06

open System


let parseLine (line: string) =
    let parts = line.Split(", ")
    int parts[0], int parts[1]

let parse (input: string) =
    let coords = input.Split('\n') |> Seq.map parseLine
    coords

type Point = int * int

type Area =
    | Number of int
    | Infinity


let addAreas a1 a2 =
    match a1, a2 with
    | Infinity, _ -> Infinity
    | _, Infinity -> Infinity
    | Number n1, Number n2 -> Number(n1 + n2)

let minMax2D coords =
    let xMin = coords |> Seq.minBy fst |> fst
    let xMax = coords |> Seq.maxBy fst |> fst
    let yMin = coords |> Seq.minBy snd |> snd
    let yMax = coords |> Seq.maxBy snd |> snd
    xMin, xMax, yMin, yMax

let distance (x0, y0) (x1, y1) = (abs (x1 - x0)) + (abs (y1 - y0))

let generatePointsInRange xMin xMax yMin yMax =
    seq {
        for x in xMin..xMax do
            for y in yMin..yMax do
                yield x, y
    }

let isAtEdge width height (x, y) =
    x = 0 || y = 0 || x = width + 1 || y = height + 1


let belongsTo (coords: seq<Point>) point =
    coords
    |> Seq.fold
        (fun (minDist, matches) coordinate ->
            match distance point coordinate with
            | dist when dist < minDist -> dist, [ coordinate ]
            | dist when dist = minDist -> minDist, coordinate :: matches
            | _ -> minDist, matches)
        (Int32.MaxValue, [])
    |> snd


let findArea (coords: seq<Point>) =
    let xMin, xMax, yMin, yMax = minMax2D coords

    let isAtEdge (x, y) =
        x = xMin || y = yMin || x = xMax || y = yMax

    generatePointsInRange xMin xMax yMin yMax
    |> Seq.map (fun point ->
        match belongsTo coords point with
        | [ coordinate ] -> Some(coordinate, point)
        | _ -> None)
    |> Seq.fold
        (fun areas opt ->
            match opt with
            | Some (ownedBy, point) ->

                let incrementBy =
                    if isAtEdge point then
                        Infinity
                    else
                        Number 1


                match Map.tryFind ownedBy areas with
                | Some area ->
                    let newArea = addAreas incrementBy area
                    Map.add ownedBy newArea areas
                | None -> Map.add ownedBy (Number 1) areas
            | _ -> areas)
        Map.empty

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let coords = parse input

            let largestArea =
                findArea coords
                |> Map.values
                |> Seq.choose (fun area ->
                    match area with
                    | Infinity -> None
                    | Number number -> Some number)
                |> Seq.max

            largestArea |> box


        member this.PartTwo(input: string) : obj =
            let coords = parse input
            let threshold = 10_000
            let xMin, xMax, yMin, yMax = minMax2D coords

            let regionSize =
                generatePointsInRange xMin xMax yMin yMax
                |> Seq.map (fun point -> coords |> Seq.map (distance point) |> Seq.sum)
                |> Seq.filter (fun sumDist -> sumDist < threshold)
                // Just assume that all of these points are connected, which turned out to work for my input
                |> Seq.length

            regionSize |> box
