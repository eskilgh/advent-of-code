namespace FSharpSolutions.Y2019.D10

open System

type Solver() =
    let parse (input: string) =
        input.Split('\n')
        |> Seq.mapi (fun y line -> line |> Seq.mapi (fun x c -> x, y, c))
        |> Seq.concat
        |> Seq.filter (fun (_, _, c) -> c = '#')
        |> Seq.map (fun (x, y, _) -> x, y)

    let subtractTpl (x1: int, y1: int) (x2: int, y2: int) = (x1 - x2, y1 - y2)

    let clockwiseAngle (u_x, u_y) (v_x, v_y) =
        let dotProduct = u_x * v_x + u_y * v_y
        let crossProduct = u_x * v_y - u_y * v_x

        // rounding the angle might be required for some inputs, but not for mine
        match atan2 (float crossProduct) (float dotProduct) with
        | ang when ang < 0.0 -> ang + Math.Tau
        | ang when ang >= Math.Tau -> ang - Math.Tau
        | ang -> ang

    let numVisible fromPoint points =
        points
        |> Seq.filter (fun point -> point <> fromPoint)
        |> Seq.map (fun point ->
            let direction = subtractTpl point fromPoint
            let up = (0, -1)
            clockwiseAngle up direction)
        |> Seq.distinct
        |> Seq.length

    let findOptimalStationLocation asteroids =
        asteroids
        |> Seq.map (fun ((x, y)) -> (x, y), numVisible (x, y) asteroids)
        |> Seq.maxBy (fun (_, visibleAsteroids) -> visibleAsteroids)


    let rec destroyTargets targets =
        let doLaserRotation targets =
            targets
            |> Seq.distinctBy (fun (angle, _, _) -> angle)

        seq {
            let destroyed = doLaserRotation targets

            yield! destroyed
            let targetsLeft = targets |> Seq.except destroyed

            if Seq.isEmpty targetsLeft then
                yield! []
            else
                yield! destroyTargets targetsLeft
        }

    let destroyAsteroids stationLocation asteroids =
        let orderedAsteroids =
            asteroids
            |> Seq.filter (fun p -> p <> stationLocation)
            |> Seq.map (fun p ->
                let dirX, dirY = subtractTpl p stationLocation
                let distSquared = dirX * dirX + dirY * dirY
                let angle = clockwiseAngle (0, -1) (dirX, dirY)
                angle, distSquared, p)
            |> Seq.sortWith (fun (a_angle, a_distSquared, _) (b_angle, b_distSquared, _) ->
                // Sort by angle, distance ascending
                match a_angle - b_angle with
                | 0.0 -> a_distSquared - b_distSquared
                | diff when diff > 0.0 -> 1
                | _ -> -1)

        destroyTargets orderedAsteroids
        |> Seq.map (fun (_, _, point) -> point)



    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let asteroids = parse input
            let (_, visibleAsteroids) = findOptimalStationLocation asteroids
            visibleAsteroids |> box

        member this.PartTwo(input: string) : obj =
            let asteroids = parse input
            let stationLocation, _ = findOptimalStationLocation asteroids

            destroyAsteroids stationLocation asteroids
            |> Seq.take 200
            |> Seq.last
            |> (fun (x, y) -> x * 100 + y)
            |> box
