module Pong

open IntCode
open System
open Common
open System.Collections.Generic
open System.Threading


type TileId =
    | Empty
    | Wall
    | Block
    | Paddle
    | Ball

type GameState =
    { Blocks: (int * int) Set
      Walls: (int * int) Set
      Paddle: (int * int)
      Ball: (int * int)
      BallDirection: (int * int)
      Score: int
      ComputeFn: int64 -> Output }

let (|TileId|) =
    function
    | 0 -> Empty
    | 1 -> Wall
    | 2 -> Block
    | 3 -> Paddle
    | 4 -> Ball
    | _ -> failwith "Invalid tile ID"

let paint tileId =
    match tileId with
    | TileId Wall -> '█'
    | TileId Block -> '▒'
    | TileId Paddle -> '¯'
    | TileId Ball -> '@'
    | TileId Empty
    | _ -> ' '

let isScore (x, y, _) = (x, y) = (-1, 0)

let numBalls (board: char array2d) =
    seq {
        for row in 0 .. Array2D.length1 board - 1 do
            for col in 0 .. Array2D.length2 board - 1 do
                if board.[row, col] = '@' then
                    yield row, col
                else
                    ()
    }
    |> Seq.length

let findAllOfId (id: int) tiles =
    tiles
    |> Seq.filter (fun (_, _, id') -> id' = id)
    |> Seq.map (fun (x, y, _) -> x, y)

type Pong =
    private
        { mutable board: char array2d
          mutable savedComputeFn: int64 -> Output
          mutable savedBoard: char array2d
          mutable savedScore: int
          mutable nextBallX: int
          history: Stack<GameState> }

    member this.play() =
        Console.Clear()
        this.drawGameState ()

        let rec runFrame input =
            match this.history.Peek().ComputeFn input with
            | Halted output ->
                this.updateState output |> ignore

                if this.history.Peek().Blocks.Count = 0 then
                    this.drawGameState ()
                    this.history.Peek().Score
                else
                    for _ in 0..10 do
                        this.history.Pop() |> ignore
                        this.drawGameState ()
                        Thread.Sleep(2)

                    let input = this.getUserInput ()
                    runFrame (input)
            | NeedsInput (output, nextComputeFn) ->
                //this.computeFn <- nextComputeFn
                this.updateState output nextComputeFn
                this.drawGameState ()
                runFrame (this.getUserInput ())

        runFrame (this.getUserInput ())

    member this.updateState programOutput computeFn =
        let tpls =
            programOutput
            |> Seq.map int
            |> Seq.chunkBySize 3
            |> Seq.map (fun chunk ->
                let x = chunk.[0]
                let y = chunk.[1]
                let id = chunk.[2]
                x, y, id)

        let scoreOption = tpls |> Seq.filter isScore |> Seq.tryExactlyOne

        let score =
            match scoreOption with
            | Some (_, _, score) -> score
            | None -> this.history.Peek().Score

        let tiles = tpls |> Seq.filter (not << isScore)
        let prevGameState = this.history.Peek()

        let paddle =
            tiles
            |> findAllOfId 3
            |> Seq.tryExactlyOne
            |> function
                | Some newPos -> newPos
                | None -> prevGameState.Paddle

        let ball =
            tiles
            |> findAllOfId 4
            |> Seq.tryExactlyOne
            |> function
                | Some newPos -> newPos
                | None -> prevGameState.Ball

        let crushedBlocks = tiles |> findAllOfId 0 |> Set.ofSeq

        let newGameState =
            { prevGameState with
                Blocks = (prevGameState.Blocks - crushedBlocks)
                Ball = ball
                Paddle = paddle
                Score = score
                BallDirection = (fst ball - fst prevGameState.Ball), (snd ball - snd prevGameState.Ball)

                ComputeFn = computeFn }

        this.history.Push newGameState

    member this.loadSave() =

        this.board <- this.savedBoard
        this.drawGameState ()

    member this.save() =
        this.savedBoard <- Array2D.copy this.board

    member this.drawGameState() =
        let currentState = this.history.Peek()

        for y in 0 .. Array2D.length1 this.board - 1 do
            for x in 0 .. Array2D.length2 this.board - 1 do
                let c =
                    if currentState.Walls.Contains((x, y)) then
                        paint 1
                    elif currentState.Blocks.Contains((x, y)) then
                        paint 2
                    elif currentState.Paddle = (x, y) then
                        paint 3
                    elif currentState.Ball = (x, y) then
                        paint 4
                    else
                        paint 0

                this.board.[y, x] <- c

        Console.SetCursorPosition(0, 0)

        let boardWithScore =
            $"       Score: {currentState.Score}      \n"
            + array2DToString this.board

        Console.WriteLine(boardWithScore)

    member this.getUserInput() =
        let rec getUserInputRec () =
            match Console.ReadKey().Key with
            | ConsoleKey.LeftArrow -> -1L
            | ConsoleKey.RightArrow -> 1L
            | ConsoleKey.UpArrow ->
                if this.history.Count > 1 then
                    this.history.Pop() |> ignore
                    this.drawGameState ()
                else
                    ()

                getUserInputRec ()
            | _ -> 0L

        getUserInputRec ()

    static member create opcodes =
        let firstOutput, firstComputeFn =
            match IntCode.execute opcodes with
            | Halted _ -> failwith "unexpected halt"
            | NeedsInput (output, computeFn) -> (output, computeFn)

        let board, score, gameState = Pong.toInitialGameState firstOutput firstComputeFn

        let history = new Stack<GameState>()
        history.Push(gameState)

        { board = board
          savedBoard = board
          savedScore = score
          savedComputeFn = firstComputeFn
          nextBallX = -1
          history = history }

    static member toInitialGameState programOutput computeFn =
        let tiles =
            programOutput
            |> Seq.map int
            |> Seq.chunkBySize 3
            |> Seq.map (fun chunk ->
                let x = chunk.[0]
                let y = chunk.[1]
                let id = chunk.[2]
                x, y, id)

        let scoreOption =
            tiles
            |> Seq.filter isScore
            |> Seq.map (fun (_, _, id) -> id)
            |> Seq.exactlyOne

        let tileSet =
            tiles
            |> Seq.filter (not << isScore)
            |> Seq.map (fun (x, y, z) -> (x, y), z)
            |> Map.ofSeq

        let xMin, xMax, yMin, yMax =
            tiles
            |> Seq.fold
                (fun (xMin, xMax, yMin, yMax) (x, y, _) -> min xMin x, max xMax x, min yMin y, max yMax y)
                (Int32.MaxValue, Int32.MinValue, Int32.MaxValue, Int32.MinValue)

        let width = xMax - xMin
        let height = yMax - yMin + 1 // ball can go outside frame when losing

        let map =
            Array2D.init (height) (width) (fun y x ->
                match tileSet.TryFind((x, y)) with
                | Some value -> paint value
                | None -> paint 0)

        let walls = tiles |> findAllOfId 1 |> Set.ofSeq
        let blocks = tiles |> findAllOfId 2 |> Set.ofSeq
        let paddle = tiles |> findAllOfId 3 |> Seq.exactlyOne
        let ball = tiles |> findAllOfId 4 |> Seq.exactlyOne

        let gameState =
            { Walls = walls
              Blocks = blocks
              Ball = ball
              Paddle = paddle
              Score = 0
              BallDirection = (0, 0)
              ComputeFn = computeFn }

        map, scoreOption, gameState
