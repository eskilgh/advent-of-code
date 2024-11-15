module Year2019Day14

type Ingredient = int * string

let ceilDiv numerand divisor = (numerand + divisor - 1) / divisor

let rec calcOreRequiredAlt (reactions: Map<Ingredient, Ingredient []>) ((quantity, chemical): Ingredient) =
    if chemical = "ORE" then
        quantity
    else
        reactions
        |> Map.filter (fun (q, c) _ -> c = chemical)
        |> Map.toSeq
        |> Seq.map (fun ((outputQuantity, outputChemical), inputIngredients) ->
            inputIngredients
            |> Seq.map (fun (q', c') ->
                let multiplier = ceilDiv quantity outputQuantity
                let required = calcOreRequiredAlt reactions (q', c')
                multiplier * required)
            |> Seq.sum)
        |> Seq.min

let calcOreRequired (reactions: Map<Ingredient, Ingredient []>) (ingredients: seq<Ingredient>) =
    ingredients
    |> Seq.map (fun (quantity, chemical) ->
        let matching =
            reactions
            |> Map.findKey (fun (_, c) _ -> c = chemical)

        let multiplier = ceilDiv quantity (fst matching)
        let ore = reactions.[matching].[0]
        let oreRequired = (fst ore) * multiplier
        oreRequired)
    |> Seq.sum


let rec expandPrimitives (reactions: Map<Ingredient, Ingredient []>) (ingredients: seq<Ingredient>) =
    let expandIngredient ingredient =
        let matching =
            reactions
            |> Map.findKey (fun (q, c) _ -> c = (snd ingredient))

        let multiplier = ceilDiv (fst ingredient) (fst matching)

        let inputs =
            reactions.[matching]
            |> Array.map (fun (q, c) -> (q * multiplier, c))

        let isPrimitiveIngredient = (snd inputs.[0]) = "ORE"

        if isPrimitiveIngredient then
            [ ingredient ]
        else
            expandPrimitives reactions inputs

    let ans = ingredients |> Seq.collect expandIngredient
    ans |> Seq.toList

let collectPrimitives (primitives: list<Ingredient>) =
    primitives
    |> List.fold
        (fun acc ((quantity, chemical): Ingredient) ->
            let existingQuantity =
                if Map.containsKey chemical acc then
                    acc.[chemical]
                else
                    0

            acc
            |> Map.add chemical (quantity + existingQuantity))
        Map.empty
    |> Map.toList
    |> List.map (fun (x, y) -> (y, x): Ingredient)

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let reactions =
                input.Split('\n')
                |> Seq.map (fun line ->
                    line
                        .Replace(",", "")
                        .Replace(" =>", "")
                        .Split(" ")
                    |> Array.chunkBySize 2
                    |> Array.map (fun (chunk) ->
                        (match chunk with
                         | [| rawQuantity; chemical |] -> (int rawQuantity, chemical): Ingredient
                         | _ -> failwith "Failed parsing input")))
                |> Seq.fold
                    (fun acc parts ->
                        let key = parts.[parts.Length - 1]
                        let value = parts.[0 .. parts.Length - 2]
                        Map.add key value acc)
                    Map.empty

            let target = (1, "FUEL")
            let primitives = expandPrimitives reactions [ target ]
            let primitiveCount = collectPrimitives primitives
            let oreRequired = calcOreRequired reactions primitiveCount
            oreRequired

        member this.PartTwo(input: string) : obj = "Not available"
