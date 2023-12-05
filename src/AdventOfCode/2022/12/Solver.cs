namespace AdventOfCode.Y2022.D12;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var heightMap = input.Split("\n").Select(s => s.ToCharArray()).ToArray();

        var start = (
            from row in heightMap.Select((cs, i) => (cs, i))
            from col in row.cs.Select((c, j) => (c, j))
            where col.c == 'S'
            select (col.c, row.i, col.j)
        ).First();

        return FindShortestPathLength(heightMap, new Position(start.i, start.j), ascending: true).ToString();
    }

    public static int FindShortestPathLength(char[][] heightMap, Position start, bool ascending)
    {
        var steps = Enumerable
            .Range(0, heightMap.Length)
            .Select(_ => Enumerable.Range(0, heightMap[0].Length).Select(_ => -1).ToArray())
            .ToArray();

        var queue = new PriorityQueue<Position, int>();
        queue.Enqueue(start, 0);

        var visitedPositions = new HashSet<Position> { start };

        while (queue.TryDequeue(out var currentPos, out var currentDistance))
        {
            steps[currentPos.Row][currentPos.Col] = currentDistance;
            var eligibleNextSteps = EligiblePositions(heightMap, currentPos, ascending);
            foreach (var position in eligibleNextSteps)
            {
                if (visitedPositions.Contains(position))
                    continue;

                visitedPositions.Add(position);
                if (ascending ? heightMap[position.Row][position.Col] == 'E' : GetElevation(heightMap, position) == 'a')
                {
                    return currentDistance + 1;
                }

                queue.Enqueue(position, currentDistance + 1);
            }
        }

        throw new ArgumentException("Could not find valid path.");
    }

    private static IList<(int RowInc, int ColInc)> Directions =>
        new List<(int RowInc, int ColInc)> { (-1, 0), (0, 1), (1, 0), (0, -1) };

    private static IList<Position> EligiblePositions(char[][] heightMap, Position current, bool ascending)
    {
        return Directions
            .Select(d => current with { Row = current.Row + d.RowInc, Col = current.Col + d.ColInc })
            .Where((pos) => 0 <= pos.Row && pos.Row < heightMap.Length && 0 <= pos.Col && pos.Col < heightMap[0].Length)
            .Where(
                (pos) =>
                {
                    var currentElevation = GetElevation(heightMap, current);
                    var newPosElevation = GetElevation(heightMap, pos);
                    return ascending
                        ? (newPosElevation - currentElevation <= 1)
                        : (currentElevation - newPosElevation <= 1);
                }
            )
            .ToList();
    }

    private static int GetElevation(char[][] heightMap, Position current)
    {
        var c = heightMap[current.Row][current.Col];
        return c switch
        {
            'S' => 'a',
            'E' => 'z',
            _ => c
        };
    }

    public string PartTwo(string input)
    {
        var heightMap = input.Split("\n").Select(s => s.ToCharArray()).ToArray();

        var start = (
            from row in heightMap.Select((cs, i) => (cs, i))
            from col in row.cs.Select((c, j) => (c, j))
            where col.c == 'E'
            select (col.c, row.i, col.j)
        ).First();

        return FindShortestPathLength(heightMap, new Position(start.i, start.j), ascending: false).ToString();
    }
}

public record Position(int Row, int Col);
