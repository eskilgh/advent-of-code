module Year2018Day07

open System

type Step =
    { Name: char
      RemainingTime: int
      Dependencies: list<char> }
    member this.Completed = this.RemainingTime = 0

type WorkerState =
    | Available
    | WorkingOn of char

type State =
    { Workers: list<WorkerState>
      Steps: array<Step>
      Tick: int }

let parseSteps (baseTimePerStep: int) (durationIncrement: int) (input: string) =
    let stepAndDependants =
        input.Split('\n')
        |> Array.map (fun line ->
            let step = line[5]
            let dependendant = line[36]
            step, dependendant)

    let init =
        stepAndDependants
        |> Array.map (fst)
        |> Array.append (stepAndDependants |> Array.map snd)
        |> Array.distinct
        |> Array.map (fun step ->
            { Name = step
              RemainingTime =
                (int step) - (int 'A')
                + durationIncrement
                + baseTimePerStep
              Dependencies = [] })
        |> Array.sortBy (fun step -> step.Name)

    stepAndDependants
    |> Array.fold
        (fun (steps: array<Step>) (stepName, dependant) ->
            let idx = (int dependant) - int ('A')
            let current = steps[idx]
            let updated = { current with Dependencies = stepName :: current.Dependencies }

            Array.updateAt idx updated steps)
        init

let setupInitialState (input: string) baseDuration durationIncrement numWorkers =
    let steps = parseSteps baseDuration durationIncrement input
    let workers = [ for _ in 0 .. numWorkers - 1 -> Available ]

    { Steps = steps
      Workers = workers
      Tick = 0 }


let rec assign (workers: List<WorkerState>) (availableSteps: list<Step>) =
    match workers, availableSteps with
    | [], _
    | _, [] -> workers
    | (WorkingOn step) :: workers', _ -> (WorkingOn step) :: assign workers' availableSteps
    | (Available) :: workers', availableStep :: availableSteps' ->
        let assignedWorker = (WorkingOn availableStep.Name)
        assignedWorker :: assign workers' availableSteps'

let isStepAvailableForWork (state: State) (step: Step) =
    match step with
    | step when not step.Completed && step.Dependencies.IsEmpty ->
        let isStepAlreadyBeingWorkedOn =
            state.Workers
            |> List.exists (fun worker ->
                match worker with
                | WorkingOn stepName when stepName = step.Name -> true
                | _ -> false)

        not isStepAlreadyBeingWorkedOn
    | _ -> false

let assignWorkers (state: State) =
    let availableSteps =
        state.Steps
        |> Array.filter (isStepAvailableForWork state)
        // probably not needed?
        |> Array.sortBy (fun step -> step.Name)
        |> Array.toList

    let assignedWorkers = assign state.Workers availableSteps
    { state with Workers = assignedWorkers }

let freeWorkers (state: State) =
    state.Workers
    |> List.map (fun worker ->
        match worker with
        | WorkingOn stepName ->
            let idx = int (stepName) - int ('A')

            if state.Steps[idx].Completed then
                Available
            else
                WorkingOn stepName
        | Available -> Available)

let rec removeIfPresent x xs =
    match xs with
    | [] -> xs
    | hd :: xs' when hd = x -> xs'
    | hd :: xs' -> hd :: removeIfPresent x xs'

let workOnStep state stepName =
    let idx = int (stepName) - int ('A')
    let current = state.Steps[idx]
    let workedStep = { current with RemainingTime = current.RemainingTime - 1 }

    let withWorkedStep = state.Steps |> Array.updateAt idx workedStep

    if workedStep.Completed then
        let withoutCompletedDependency =
            withWorkedStep
            |> Array.map (fun step -> { step with Dependencies = removeIfPresent workedStep.Name step.Dependencies })

        { state with
            Steps = withoutCompletedDependency
            Workers = freeWorkers state }
    else
        { state with Steps = withWorkedStep }

let work (state: State) =
    state.Workers
    |> List.choose (fun worker ->
        match worker with
        | WorkingOn stepName -> Some stepName
        | Available -> None)
    |> List.fold workOnStep state
    |> (fun state ->
        { state with
            Workers = (freeWorkers state)
            Tick = state.Tick + 1 })

let doOneTick = assignWorkers >> work


let completeInOrder (state: State) =
    let rec inner completed state =
        let state' = doOneTick state

        let completed' =
            state'.Steps
            |> Array.filter (fun step ->
                step.Completed
                && not (Array.contains step.Name completed))
            |> Array.map (fun step -> step.Name)
            |> Array.append completed

        let allCompleted =
            state'.Steps
            |> Array.forall (fun step -> step.Completed)

        if allCompleted then
            completed'
        else
            inner completed' state'

    inner [||] state

let rec workTilCompletion (state: State) =
    if state.Steps
       |> Array.forall (fun step -> step.Completed) then
        state
    else
        state |> doOneTick |> workTilCompletion

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let baseDuration = 1
            let durationIncrement = 0
            let numWorkers = 1
            let state = setupInitialState input baseDuration durationIncrement numWorkers

            completeInOrder state |> String.Concat |> box



        member this.PartTwo(input: string) : obj =
            let baseDuration = 60
            let durationIncrement = 1
            let numWorkers = 5
            let state = setupInitialState input baseDuration durationIncrement numWorkers

            let finalState = workTilCompletion state
            finalState.Tick |> box
