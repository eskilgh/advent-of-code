module Year2019Day06

type Solver() =
    let countOrbits (entities: Map<string, string list>) =
        let rec countOrbitsRec (numParents: int) (body: string) =
            match entities.TryFind(body) with
            | None -> numParents
            | Some orbitingBodies ->
                numParents
                + (orbitingBodies
                   |> List.sumBy (countOrbitsRec (numParents + 1)))

        countOrbitsRec 0 "COM"

    let pathTo (bodies: Map<string, string>) ancestor name =
        let rec pathToRec name' =
            match bodies.TryFind(name') with
            | Some parent ->
                if parent = ancestor then
                    [ parent ]
                else
                    parent :: (pathToRec parent)
            | None -> failwith "error"

        pathToRec name

    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let bodies =
                input.Split('\n')
                |> Seq.map (fun line ->
                    let segments = line.Split(')')
                    segments[0], segments[1])

            let folder (entities: Map<string, string list>) ((parentBody, orbiter): (string * string)) =
                match entities.TryFind(parentBody) with
                | Some orbitingBodies ->
                    entities
                    |> Map.add parentBody (orbiter :: orbitingBodies)
                | None -> entities |> Map.add parentBody [ orbiter ]

            let totalNumberOfOrbits = bodies |> Seq.fold folder Map.empty |> countOrbits
            totalNumberOfOrbits |> box

        member this.PartTwo(input: string) : obj =
            let bodies =
                input.Split('\n')
                |> Seq.map (fun line ->
                    let segments = line.Split(')')
                    (segments[1], segments[0]))
                |> Map.ofSeq

            let pathA = pathTo bodies "COM" "YOU"
            let pathB = pathTo bodies "COM" "SAN"

            let lengthFromRootToFirstCommonParent =
                Seq.zip (pathA |> List.rev) (pathB |> List.rev)
                |> Seq.takeWhile (fun (a, b) -> a = b)
                |> Seq.length

            let transfersRequired =
                pathA.Length + pathB.Length
                - (2 * lengthFromRootToFirstCommonParent)

            transfersRequired |> box
