namespace FSharpSolutions.Y2019.D03

type Solver() =
    let (|Prefix|_|) (p:string) (s:string) =
        if s.StartsWith(p) then
            Some(s.Substring(p.Length))
        else
            None

    interface Shared.ISolver with
        member this.PartOne(input: string): obj =
            let centralPort = (0, 0);
            let rec getPositions (position: int * int) (instructions: (char * int) list) =
                match instructions with
                | [] -> []
                | (direction, distance)::tl -> 
                    let nextPosition = 
                        let scalarMultiply (a,b) scalar = (a * scalar, b * scalar)
                        let add (a: int * int) (b: int * int) = (fst a + fst b, snd a + snd b)
                        let u = 
                            match direction with
                            | 'U' -> (-1, 0)
                            | 'D' -> (1, 0)
                            | 'L' -> (0, -1)
                            | 'R' -> (0, 1)
                            | c -> failwith $"Unexpected direction {c}"
                        add position (scalarMultiply u distance)
                    nextPosition :: (getPositions nextPosition tl)

            let instructions = 
                input.Split('\n') 
                |> Array.toList
                |> (List.map (fun line -> 
                    line.Split(',')
                    |> Array.toList
                    |> List.map (fun (instruction) ->
                        let direction = instruction.[0]
                        let distance = int (instruction.[1..])
                        (direction, distance))))
            
            let positions =
                instructions
                |> List.map (getPositions centralPort)

            let lines : ((int * int) * (int * int)) list list= 
                positions
                |> List.map List.pairwise

            let findIntersection (a: ((int * int) * (int * int))) (b: ((int * int) * (int * int))) =
                // Sort points of line segment 'a' by their x-coordinate
                let ((ax1, ay1), (ax2, ay2)) = 
                    let p1, p2 = a
                    (min p1 p2), (max p1 p2)
        
                // Sort points of line segment 'b' by their x-coordinate
                let ((bx1, by1), (bx2, by2)) = 
                    let p1, p2 = b
                    (min p1 p2), (max p1 p2)
                
                 // Check if rectangles intersect
                let intersects =
                    ax1 <= bx2 && ax2 >= bx1 && ay1 <= by2 && ay2 >= by1

                // Calculate intersection point
                if intersects then
                    // Determine if intersection is horizontal or vertical
                    if ax1 <> ax2 then
                        Some (bx1, ay1)
                    else
                        Some (ax1, by1)
                else
                    None
                
            let fst::snd::_ = lines;

            let intersections = 
                seq {
                    for u in fst do
                        for v in snd do
                            match findIntersection u v with
                            | Some point -> yield point
                            | None -> ()
                }

            let distance (x0, y0) (x1, y1) = abs (x1 - x0) + abs (y1 - y0)

            intersections |> Seq.map (distance (0, 0)) |> Seq.min |> box

        member this.PartTwo(input: string): obj =
            "Not available"