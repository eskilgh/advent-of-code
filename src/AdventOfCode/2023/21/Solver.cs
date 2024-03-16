using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode.Y2023.D21;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return "nevermimnd";
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        var height = m.Length;
        var width = m[0].Length;
        var (startRow, startCol) = m.IndexOf2d('S');
        var start = new Vector2d(x: startCol, y: startRow);
        var seen = new Dictionary<Vector2d, int>();
        var queue = new Queue<(Vector2d, int step)>();
        seen[start] = 0;
        queue.Enqueue((start, 0));
        Vector2d[] directions = [Vector2d.Up, Vector2d.Right, Vector2d.Down, Vector2d.Left];

        var numSteps = 64;
        var numStepsModTwo = numSteps % 2;
        var loggedSteps = new HashSet<int>();
        while (queue.Count > 0)
        {
            var (pos, steps) = queue.Dequeue();
            //if (steps % 10 == 0 && loggedSteps.Add(steps))
            //    Print(m, seen.Where(kvp => kvp.Value % 2 == numStepsModTwo).Select(kvp => kvp.Key).ToHashSet());
            if (steps == numSteps)
                continue;
            var newPositions = directions
                .Select(dir => pos + dir)
                .Where(newPos =>
                {
                    var isValid = !newPos.IsOutOfBounds(xMin: 0, xMax: width - 1, yMin: 0, yMax: height - 1)
                                  && !seen.ContainsKey(newPos) && m[newPos.Y][newPos.X] == '.';
                    return isValid;
                });
            foreach (var newPos in newPositions)
            {
                seen[newPos] = steps + 1;
                queue.Enqueue((newPos, steps + 1));
            }
        }

        var numGardenPlots = m.Sum(cs => cs.Count(c => c is '.' or 'S'));
        var possibleLocationsAfterNSteps =
            seen.Where(kvp => kvp.Value % 2 == numStepsModTwo).Select(kvp => kvp.Key).ToHashSet();
        return possibleLocationsAfterNSteps.Count;
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        var height = m.Length;
        var width = m.Length;
        var (startRow, startCol) = m.IndexOf2d('S');

        var numSteps = 64;

        int[][] matrix =
        [
            [6, 5, 4, 3, 4, 5, 6],
            [5, 4, 3, 2, 3, 4, 5],
            [4, 3, 2, 1, 2, 3, 4],
            [3, 2, 1, 0, 1, 2, 3],
            [4, 3, 2, 1, 2, 3, 4],
            [5, 4, 3, 2, 3, 4, 5],
            [6, 5, 4, 3, 4, 5, 6]
        ];

        var positions = GetPositions(
            width: width,
            height: height,
            startRow: startRow,
            startCol: startCol,
            numSteps: numSteps,
            infiniteGrid: false);

        var count = 0;
        foreach (var pos in positions)
        {
            var c = m[pos.row][pos.col];
            if (c is '.' or 'S')
                count++;
        }

        return positions.Count(pos => m[pos.row][pos.col] is '.' or 'S');


        return "Not available";
    }

    static IEnumerable<(int row, int col)> GetPositions(int width, int height, int startRow, int startCol, int numSteps,
        bool infiniteGrid)
    {
        if (numSteps < 0) throw new ArgumentOutOfRangeException(nameof(numSteps));

        var numStepsModTwo = numSteps % 2;
        if (numStepsModTwo == 0)
        {
            yield return (startRow, startCol);
        }
        else
        {
            yield return (startRow, startCol - 1);
            yield return (startRow, startCol + 1);
        }

        for (var step = numStepsModTwo + 2; step <= numSteps; step += 2)
        {
            for (var i = 0; i <= step; i++)
            {
                var upperLeft = (startRow - i, startCol - step + i);
                var upperRight = (startRow - i, startCol + step - i);
                var lowerLeft = (startRow + i, startCol - step + i);
                var lowerRight = (startRow + i, startCol + step - i);

                (int row, int col)[] positions = [upperLeft, upperRight, lowerLeft, lowerRight];

                if (!infiniteGrid)
                {
                    foreach (var pos in positions.Where(pos =>
                                 0 < pos.row && pos.row < height && 0 <= pos.col && pos.col < width))
                        yield return pos;
                }
                else
                {
                    foreach (var (row, col) in positions)
                    {
                        yield return (Mod(row, height), Mod(col, width));
                    }
                }

            }
        }
    }

    static int Mod(int x, int m) =>
        (x % m + m) % m;

    public object PartTwoAlt(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        var height = m.Length;
        var width = m.Length;

        var (startRow, startCol) = m.IndexOf2d('S');
        var start = new Vector2d(x: startCol, y: startRow);
        var baseMap = new Dictionary<Vector2d, int>();
        var queue = new Queue<(Vector2d, int step)>();
        Vector2d[] directions = [Vector2d.Up, Vector2d.Right, Vector2d.Down, Vector2d.Left];

        baseMap[start] = 0;
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (pos, steps) = queue.Dequeue();

            var validNextPositions = directions
                .Select(dir => pos + dir)
                .Where(newPos =>
                {
                    var isValid = !newPos.IsOutOfBounds(xMin: 0, xMax: width - 1, yMin: 0, yMax: height - 1)
                                  && !baseMap.ContainsKey(newPos) && m[newPos.Y][newPos.X] == '.';
                    return isValid;
                });

            foreach (var position in validNextPositions)
            {
                baseMap[position] = steps + 1;
                queue.Enqueue((position, steps + 1));
            }
        }

        var pqueue = new PriorityQueue<(Vector2d, int), int>();
        var nextTraversal = new Dictionary<Vector2d, int>();

        Vector2d[] startPositions =
        [
            new(x: startCol, y: 0), new(x: startCol, y: height - 1),
            new(x: 0, y: startRow), new(x: width - 1, y: startRow)
        ];

        foreach (var pos in startPositions)
        {
            nextTraversal[pos] = 1;
            pqueue.Enqueue((pos, 1), 1);
        }

        while (pqueue.Count > 0 && nextTraversal.Count < baseMap.Count)
        {
            var (pos, steps) = pqueue.Dequeue();

            var validNextPositions = directions
                .Select(dir => pos + dir)
                .Where(newPos =>
                {
                    var isValid = !newPos.IsOutOfBounds(xMin: 0, xMax: width - 1, yMin: 0, yMax: height - 1)
                                  && !nextTraversal.ContainsKey(newPos) && m[newPos.Y][newPos.X] == '.';
                    return isValid;
                });

            foreach (var position in validNextPositions)
            {
                nextTraversal[position] = steps + 1;
                pqueue.Enqueue((position, steps + 1), steps + 1);
            }
        }

        Print(m, baseMap, @"C:\Users\n653917\repos\advent-of-code\src\AdventOfCode\2023\21\basemap.txt");
        Print(m, nextTraversal, @"C:\Users\n653917\repos\advent-of-code\src\AdventOfCode\2023\21\nextTraversal.txt");

        var numSteps = 26501365;
        var gardenPlotsPerGrid = baseMap.Count;
        var stepsFromBaseToNextGrid = ((height - 1) / 2) + 1;
        var stepsFromNextGridToNextGrid = height;


        return "Not available";
    }

    void Print(char[][] m, IDictionary<Vector2d, int> visited, string? path = default)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                var key = new Vector2d(col, row);
                var c = visited.TryGetValue(key, out var steps) ? $"{steps} " : $"{m[row][col]} ";
                sb.Append(c);
            }

            sb.AppendLine();
        }

        if (path is not null)
        {
            File.WriteAllText(path, sb.ToString());
        }
        //Console.WriteLine(sb.ToString());
    }

    void Print(char[][] m, ISet<Vector2d> visited)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                sb.Append(visited.Contains(new Vector2d(col, row)) ? 'O' : m[row][col]);
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
}