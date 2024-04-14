module Year2019Day12

open System.Collections.Generic
open Common

type Vector3D =
    { X: int
      Y: int
      Z: int }
    member this.Magnitude = abs this.X + abs this.Y + abs this.Z

    static member (+)(u: Vector3D, v: Vector3D) =
        { X = u.X + v.X
          Y = u.Y + v.Y
          Z = u.Z + v.Z }

    static member (-)(u: Vector3D, v: Vector3D) =
        { X = u.X - v.X
          Y = u.Y - v.Y
          Z = u.Z - v.Z }

type Moon =
    { Position: Vector3D
      Velocity: Vector3D }
    member this.PotentialEnergy = this.Position.Magnitude
    member this.KineticEnergy = this.Velocity.Magnitude
    member this.Energy = this.PotentialEnergy * this.KineticEnergy

    override this.ToString() =
        $"pos=<x={this.Position.X}, y={this.Position.Y}, z={this.Position.Z}>, vel=<x={this.Velocity.X}, y={this.Velocity.Y}, z={this.Velocity.Z}>"

let parseLine (line: string) =
    let parts =
        line.Split([| '<'; '>'; ','; '=' |], System.StringSplitOptions.RemoveEmptyEntries)

    let x, y, z = int parts.[1], int parts.[3], int parts.[5]

    { Position = { X = x; Y = y; Z = z }
      Velocity = { X = 0; Y = 0; Z = 0 } }

let parse (input: string) =
    input.Split('\n')
    |> Seq.map parseLine
    |> List.ofSeq

let toSignVector v =
    { X = sign v.X
      Y = sign v.Y
      Z = sign v.Z }

let advanceTime1D (states: (int * int) list) =
    let applyGravity1D state =
        states
        |> List.filter (fun x -> x <> state)
        |> List.fold (fun (pos, vel) (currPos, _) -> (pos, vel + sign (currPos - pos))) state

    states
    |> List.map applyGravity1D
    |> List.map (fun (pos, vel) -> (pos + vel, vel))

let advanceTime moons =
    let applyGravity moon =
        moons
        |> List.filter (fun m -> m <> moon)
        |> List.fold
            (fun moon' m ->
                let signV = m.Position - moon.Position |> toSignVector
                { moon' with Velocity = moon'.Velocity + signV })
            moon

    moons
    |> List.map applyGravity
    |> List.map (fun moon -> { moon with Position = moon.Position + moon.Velocity })

let advanceNSteps n moons =
    let rec loop n' moons =
        if n' = n then
            moons
        else
            let newMoons = advanceTime moons
            loop (n' + 1) newMoons

    loop 0 moons

let advanceUntilRepeat1D moons selector =
    let rec loop state (seen: HashSet<(int * int) list>) =
        let next = advanceTime1D state

        if seen.Contains(next) then
            seen.Count
        else
            seen.Add(next) |> ignore
            loop next seen

    let startState = moons |> List.map selector
    let seen = new HashSet<(int * int) list>()
    seen.Add(startState) |> ignore
    loop startState seen

let stepsUntilRepeat moons =
    let selectors =
        [ (fun m -> m.Position.X, m.Velocity.X)
          (fun m -> m.Position.Y, m.Velocity.Y)
          (fun m -> m.Position.Z, m.Velocity.Z) ]

    selectors
    |> List.map (fun s -> advanceUntilRepeat1D moons s)
    |> List.map int64
    |> List.fold lcm 1

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let moons = parse input

            moons
            |> advanceNSteps 1000
            |> List.fold (fun acc moon -> acc + moon.Energy) 0
            |> box

        member this.PartTwo(input: string) : obj =
            let moons = parse input

            moons |> stepsUntilRepeat |> box
