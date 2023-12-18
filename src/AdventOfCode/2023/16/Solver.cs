using System.Text;
using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D16;

internal class Solver : ISolver
{
    public static int count = 0;

    public string PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var direction = new Vector2d(1, 0);
        var pos = new Vector2d(0, 0);
        var visitedPositions = TraverseRec(m, pos, direction);

        return visitedPositions.Count().ToString();
    }

    public string PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        Vector2d[] directions = [Vector2d.Down, Vector2d.Up, Vector2d.Left, Vector2d.Right];
        var height = m.Length;
        var width = m[0].Length;
        return directions
            .SelectMany(
                direction =>
                    direction switch
                    {
                        var d when d == Vector2d.Up
                            => Enumerable
                                .Range(0, width)
                                .Select((i) => TraverseIter(m, new Vector2d(i, height - 1), Vector2d.Up).Count),
                        var d when d == Vector2d.Down
                            => Enumerable
                                .Range(0, width)
                                .Select((i) => TraverseIter(m, new Vector2d(i, 0), Vector2d.Down).Count),
                        var d when d == Vector2d.Left
                            => Enumerable
                                .Range(0, height)
                                .Select((i) => TraverseIter(m, new Vector2d(width - 1, i), Vector2d.Left).Count),
                        var d when d == Vector2d.Right
                            => Enumerable
                                .Range(0, height)
                                .Select((i) => TraverseIter(m, new Vector2d(0, i), Vector2d.Right).Count),
                        _ => throw new ArgumentOutOfRangeException()
                    }
            )
            .Max()
            .ToString();
    }

    static ISet<Vector2d> TraverseIter(char[][] m, Vector2d startPosition, Vector2d startDirection)
    {
        var seen = new HashSet<(int, int, int, int)>();
        var stack = new Stack<(Vector2d, Vector2d)>();
        stack.Push((startPosition, startDirection));

        while (stack.Count > 0)
        {
            var (pos, direction) = stack.Pop();
            if (pos.X < 0 || m[0].Length <= pos.X || pos.Y < 0 || m.Length <= pos.Y)
                continue;
            if (!seen.Add((pos.X, pos.Y, direction.X, direction.Y)))
                continue;

            var current = m[pos.Y][pos.X];
            List<Vector2d> newDirections = (current) switch
            {
                '.' => [direction],
                '/' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Left()],
                '/' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Right()],
                '\\' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Right()],
                '\\' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Left()],
                '|' when direction.Abs() == new Vector2d(0, 1) => [direction],
                '-' when direction.Abs() == new Vector2d(1, 0) => [direction],
                '|' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Right(), direction.Rotate90Left()],
                '-' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Right(), direction.Rotate90Left()],
                _ => throw new ArgumentOutOfRangeException()
            };

            foreach (var newDirection in newDirections)
                stack.Push(((pos + newDirection), newDirection));
        }
        return seen.Select(tpl => new Vector2d(tpl.Item1, tpl.Item2)).ToHashSet();
    }

    static ISet<Vector2d> TraverseRec(char[][] m, Vector2d pos, Vector2d direction)
    {
        var seen = new HashSet<(int, int, int, int)>();
        TraverseRec(m, pos, direction, seen);
        return seen.Select(tpl => new Vector2d(tpl.Item1, tpl.Item2)).ToHashSet();
    }

    static void TraverseRec(char[][] m, Vector2d pos, Vector2d direction, ISet<(int, int, int, int)> seen)
    {
        if (pos.X < 0 || m[0].Length <= pos.X || pos.Y < 0 || m.Length <= pos.Y)
            return;
        if (!seen.Add((pos.X, pos.Y, direction.X, direction.Y)))
            return;

        var current = m[pos.Y][pos.X];
        List<Vector2d> newDirections = (current) switch
        {
            '.' => [direction],
            '/' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Left()],
            '/' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Right()],
            '\\' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Right()],
            '\\' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Left()],
            '|' when direction.Abs() == new Vector2d(0, 1) => [direction],
            '-' when direction.Abs() == new Vector2d(1, 0) => [direction],
            '|' when direction.Abs() == new Vector2d(1, 0) => [direction.Rotate90Right(), direction.Rotate90Left()],
            '-' when direction.Abs() == new Vector2d(0, 1) => [direction.Rotate90Right(), direction.Rotate90Left()],
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var newDirection in newDirections)
        {
            TraverseRec(m, pos + newDirection, newDirection, seen);
        }
    }

    void Print(char[][] m, ISet<Vector2d> visited)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                sb.Append(visited.Contains(new Vector2d(col, row)) ? '#' : m[row][col]);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
}
