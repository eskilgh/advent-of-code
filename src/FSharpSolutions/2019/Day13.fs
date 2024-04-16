module Year2019Day13

open IntCode
open Pong

type TileId =
    | Empty = 0
    | Wall = 1
    | Block = 2
    | Paddle = 3
    | Ball = 4

type GameState =
    { Blocks: (int * int) Set
      Walls: (int * int) Set
      Paddle: (int * int)
      BallPos: (int * int)
      BallVel: (int * int)
      Score: int }

let isScore (x, y, _) = (x, y) = (-1, 0)

let toTileId (n: int) =
    match n with
    | 0 -> TileId.Empty
    | 1 -> TileId.Wall
    | 2 -> TileId.Block
    | 3 -> TileId.Paddle
    | 4 -> TileId.Ball
    | _ -> failwith "Invalid tile ID"


let parseGameState output prevState =
    let chunks =
        output
        |> Seq.map int
        |> Seq.chunkBySize 3
        |> Seq.map (fun chunk ->
            let x = chunk.[0]
            let y = chunk.[1]
            let id = chunk.[2]
            x, y, id)

    let score =
        chunks
        |> Seq.filter isScore
        |> Seq.map (fun (_, _, id) -> id)
        |> Seq.tryExactlyOne
        |> fun scoreOpt -> defaultArg scoreOpt prevState.Score

    let tiles =
        chunks
        |> Seq.filter (not << isScore)
        |> Seq.map (fun (x, y, id) -> x, y, enum<TileId> id)

    let findAllOf (tileId) =
        tiles
        |> Seq.filter (fun (_, _, id) -> id = tileId)
        |> Seq.map (fun (x, y, _) -> x, y)


    let blocks =
        // parsing initial game state
        if prevState.Blocks.IsEmpty then
            findAllOf TileId.Block |> Set.ofSeq
        else
            let crushedBlocks = findAllOf TileId.Empty |> Set.ofSeq
            prevState.Blocks - crushedBlocks

    let walls =
        // parsing initial game state
        if prevState.Walls.IsEmpty then
            findAllOf TileId.Wall |> Set.ofSeq
        else
            prevState.Walls

    { Walls = walls
      Blocks = blocks
      Paddle =
        findAllOf TileId.Paddle
        |> Seq.tryExactlyOne
        |> fun paddleOpt -> defaultArg paddleOpt prevState.Paddle

      BallPos =
          findAllOf TileId.Ball
          |> Seq.tryExactlyOne
          |> fun paddleOpt -> defaultArg paddleOpt prevState.BallPos
      BallVel = 0, 0
      Score = score }

let parseInitialGameState output =
    parseGameState
        output
        { Walls = Set.empty
          Blocks = Set.empty
          Paddle = 0, 0
          BallPos = 0, 0
          BallVel = 0, 0
          Score = 0 }

let trackBall state =
    let nextBallX = fst state.BallPos + fst state.BallVel
    let paddleX = fst state.Paddle

    if paddleX < nextBallX then 1L
    elif paddleX > nextBallX then -1L
    else 0L


let rec play state computeFn =
    let input = trackBall state

    match computeFn input with
    | Halted output -> parseGameState output state
    | NeedsInput (output, nextComputeFn) ->
        let nextState = parseGameState output state
        play nextState nextComputeFn


type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64

            let output =
                match IntCode.execute opcodes with
                | Halted output -> output
                | NeedsInput _ -> failwith "Expected program to halt"

            let state = parseInitialGameState output
            state.Blocks |> Seq.length |> box

        member this.PartTwo(input: string) : obj =
            let opcodes = input.Split(',') |> Array.map int64
            opcodes.[0] <- 2

            // set this to true to play a Console version of the game
            let playManually = false

            if playManually then
                let pong = Pong.create opcodes
                pong.play () |> box
            else
                match IntCode.execute opcodes with
                | Halted _ -> failwith "unexpected halt"
                | NeedsInput (output, computeFn) ->
                    let initialGameState = parseInitialGameState output
                    let finalState = play initialGameState computeFn
                    finalState.Score |> box
