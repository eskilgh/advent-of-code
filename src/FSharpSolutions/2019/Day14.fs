module Year2019Day14

let ceilDiv numerand divisor = (numerand + divisor - 1) / divisor
type Ingredient = { Quantity: int; Chemical: string }


type IngredientNode(chemical: string, initialQuantity: int, recipeYield: int, recipe: Map<string, int>) =
    let mutable quantity = initialQuantity

    member this.Chemical = chemical

    member this.Quantity
        with get () = quantity
        and private set (value) = quantity <- value

    member this.RecipeYield = recipeYield
    member this.Recipe = recipe

    member this.GetLeftOver() = 
        let leftOver = this.Quantity % this.RecipeYield
        new IngredientNode(chemical = this.Chemical, initialQuantity = -leftOver, recipeYield = this.RecipeYield, recipe = this.Recipe)

    member this.IncreaseQuantity(increaseBy: int, ingredients: list<IngredientNode>) =
        let oldIngredientMultiplier = ceilDiv this.Quantity this.RecipeYield
        let newQuantity = this.Quantity + increaseBy
        let newIngredientMultiplier = ceilDiv newQuantity this.RecipeYield

        let increaseIngredientsByMultiplier =
            newIngredientMultiplier - oldIngredientMultiplier

        if increaseIngredientsByMultiplier > 0 then
            for chemical in this.Recipe.Keys do
                let multiplier =
                    this.Recipe.[chemical]
                    * increaseIngredientsByMultiplier

                let matchingIngredient =
                    ingredients
                    |> List.find (fun x -> x.Chemical = chemical)

                matchingIngredient.IncreaseQuantity(multiplier, ingredients)
        else
            ()

        this.Quantity <- newQuantity
        

let rec findFuelPattern (ingredients: list<IngredientNode>) = 
    let rec findFuelPatternRec count currentIngredients =
        if (currentIngredients |> List.forall (fun x -> x.Quantity = 0))
        then count
        else
            let fuel = currentIngredients |> List.find (fun x -> x.Chemical = "FUEL")
            fuel
    let fuel = ingredients |> List.find (fun x -> x.Chemical = "FUEL")
    fuel.IncreaseQuantity(1, ingredients)
    let leftOverIngredients = ingredients |> List.map (fun ingredient -> ingredient.GetLeftOver())
    "Not available"

type Solver() =
    interface Shared.ISolver with
        member this.PartOne(input: string) : obj =
            let reactions =
                input.Split('\n')
                |> Array.toList
                |> List.map (fun line ->
                    line
                        .Replace(",", "")
                        .Replace(" =>", "")
                        .Split(" ")
                    |> Array.toList
                    |> List.chunkBySize 2
                    |> List.map (fun (chunk) ->
                        (match chunk with
                         | [ rawQuantity; chemical ] ->
                             { Quantity = int rawQuantity
                               Chemical = chemical }: Ingredient
                         | _ -> failwith "Failed parsing input")))

                |> List.map (fun parts ->
                    let output = parts.[parts.Length - 1]
                    let inputs = parts.[0 .. parts.Length - 2]

                    let recipe =
                        inputs
                        |> List.map (fun x -> (x.Chemical, x.Quantity))
                        |> Map.ofList

                    new IngredientNode(
                        chemical = output.Chemical,
                        initialQuantity = 0,
                        recipeYield = output.Quantity,
                        recipe = recipe
                    ))
                |> List.append [ new IngredientNode(
                                     chemical = "ORE",
                                     initialQuantity = 0,
                                     recipeYield = 1,
                                     recipe = Map.empty
                                 ) ]

            let fuel =
                reactions
                |> List.find (fun x -> x.Chemical = "FUEL")

            fuel.IncreaseQuantity(1, reactions)

            let ore =
                reactions
                |> List.find (fun x -> x.Chemical = "ORE")

            ore.Quantity

        member this.PartTwo(input: string) : obj =
            let ingredients =
                input.Split('\n')
                |> Array.toList
                |> List.map (fun line ->
                    line
                        .Replace(",", "")
                        .Replace(" =>", "")
                        .Split(" ")
                    |> Array.toList
                    |> List.chunkBySize 2
                    |> List.map (fun (chunk) ->
                        (match chunk with
                         | [ rawQuantity; chemical ] ->
                             { Quantity = int rawQuantity
                               Chemical = chemical }: Ingredient
                         | _ -> failwith "Failed parsing input")))

                |> List.map (fun parts ->
                    let output = parts.[parts.Length - 1]
                    let inputs = parts.[0 .. parts.Length - 2]

                    let recipe =
                        inputs
                        |> List.map (fun x -> (x.Chemical, x.Quantity))
                        |> Map.ofList

                    new IngredientNode(
                        chemical = output.Chemical,
                        initialQuantity = 0,
                        recipeYield = output.Quantity,
                        recipe = recipe
                    ))
                |> List.append [ new IngredientNode(
                                     chemical = "ORE",
                                     initialQuantity = 0,
                                     recipeYield = 1,
                                     recipe = Map.empty
                                 ) ]

            let fuel =
                ingredients
                |> List.find (fun x -> x.Chemical = "FUEL")
            
            let pattern = findFuelPattern ingredients

            fuel.IncreaseQuantity(1, ingredients)
