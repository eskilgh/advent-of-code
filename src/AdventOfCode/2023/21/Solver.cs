using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode.Y2023.D21;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
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
            var newPositions = directions.Select(dir => pos + dir).Where(newPos =>
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
        var possibleLocationsAfterNSteps = seen.Where(kvp => kvp.Value % 2 == numStepsModTwo).Select(kvp => kvp.Key).ToHashSet();
        return possibleLocationsAfterNSteps.Count;
    }

    public object PartTwo(string input)
    {
        return "Not available";
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
