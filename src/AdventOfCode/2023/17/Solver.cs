using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D17;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.Select(c => (c - '0')).ToArray()).ToArray();

        var start = new Vector2d(0, 0);
        var target = new Vector2d(m[0].Length - 1, m.Length - 1);
        return FindPath(m, start, target, minMovesInDirection: 0, maxMovesInDirection: 3);
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.Select(c => (c - '0')).ToArray()).ToArray();

        var start = new Vector2d(0, 0);
        var target = new Vector2d(m[0].Length - 1, m.Length - 1);
        return FindPath(m, start, target, minMovesInDirection: 4, maxMovesInDirection: 10);
    }

    int FindPath(int[][] m, Vector2d start, Vector2d target, int minMovesInDirection, int maxMovesInDirection)
    {
        var pq = new PriorityQueue<(Vector2d Pos, Vector2d Dir, int MovesLeft), int>();
        var seen = new HashSet<(Vector2d, Vector2d, int)>();
        const int heuristicMultiplier = 1;
        int CalcHeuristic(int row, int col) =>
            target.ManhattanDistance(new Vector2d(x: col, y: row)) * heuristicMultiplier;

        var heuristics = m.Select((row, rowIdx) => row.Select((_, colIdx) => CalcHeuristic(rowIdx, colIdx)).ToArray())
            .ToArray();
        pq.Enqueue((start, Vector2d.Right, maxMovesInDirection - 1), 0 + heuristics[start.Y][start.X]);
        seen.Add((start, Vector2d.Right, maxMovesInDirection - 1));
        pq.Enqueue((start, Vector2d.Down, maxMovesInDirection - 1), 0 + heuristics[start.Y][start.X]);
        seen.Add((start, Vector2d.Down, maxMovesInDirection - 1));
        while (pq.TryDequeue(out var curr, out int priority))
        {
            var (pos, direction, movesLeft) = curr;
            var heatLoss = priority - heuristics[pos.Y][pos.X];

            if (pos == target && movesLeft < (maxMovesInDirection - minMovesInDirection))
                return heatLoss;

            (Vector2d, int)[] directions = movesLeft switch
            {
                var n when n > (maxMovesInDirection - minMovesInDirection) => [(direction, movesLeft)],
                0
                    =>
                    [
                        (direction.Rotate90Left(), maxMovesInDirection),
                        (direction.Rotate90Right(), maxMovesInDirection)
                    ],
                _
                    =>
                    [
                        (direction.Rotate90Left(), maxMovesInDirection),
                        (direction, movesLeft),
                        (direction.Rotate90Right(), maxMovesInDirection)
                    ]
            };

            foreach (var (nextDir, nextMovesLeft) in directions)
            {
                var nextPos = pos + nextDir;
                var isOutOfBounds = nextPos.X < 0 || nextPos.X >= m[0].Length || nextPos.Y < 0 || nextPos.Y >= m.Length;
                if (isOutOfBounds || !seen.Add((nextPos, nextDir, nextMovesLeft - 1)))
                    continue;
                int destHeatLoss = m[nextPos.Y][nextPos.X];
                pq.Enqueue(
                    (nextPos, nextDir, nextMovesLeft - 1),
                    heatLoss + destHeatLoss + heuristics[nextPos.Y][nextPos.X]
                );
            }
        }
        throw new ArgumentOutOfRangeException();
    }
}
