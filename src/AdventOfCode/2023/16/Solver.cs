using System.Text;

namespace AdventOfCode.Y2023.D16;

internal class Solver : ISolver
{
    public static int count = 0;

    public string PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var direction = new Vector2(1, 0);
        var pos = new Vector2(0, 0);
        var visitedPositions = TraverseRec(m, pos, direction);

        return visitedPositions.Count().ToString();
    }

    public string PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        Vector2[] directions = [Vector2.Down, Vector2.Up, Vector2.Left, Vector2.Right];
        var height = m.Length;
        var width = m[0].Length;
        return directions
            .SelectMany(
                direction =>
                    direction switch
                    {
                        var d when d == Vector2.Up
                            => Enumerable
                                .Range(0, width)
                                .Select((i) => TraverseIter(m, new Vector2(i, height - 1), Vector2.Up).Count),
                        var d when d == Vector2.Down
                            => Enumerable
                                .Range(0, width)
                                .Select((i) => TraverseIter(m, new Vector2(i, 0), Vector2.Down).Count),
                        var d when d == Vector2.Left
                            => Enumerable
                                .Range(0, height)
                                .Select((i) => TraverseIter(m, new Vector2(width - 1, i), Vector2.Left).Count),
                        var d when d == Vector2.Right
                            => Enumerable
                                .Range(0, height)
                                .Select((i) => TraverseIter(m, new Vector2(0, i), Vector2.Right).Count),
                        _ => throw new ArgumentOutOfRangeException()
                    }
            )
            .Max()
            .ToString();
    }

    static ISet<Vector2> TraverseIter(char[][] m, Vector2 startPosition, Vector2 startDirection)
    {
        var seen = new HashSet<(int, int, int, int)>();
        var stack = new Stack<(Vector2, Vector2)>();
        stack.Push((startPosition, startDirection));

        while (stack.Count > 0)
        {
            var (pos, direction) = stack.Pop();
            if (pos.X < 0 || m[0].Length <= pos.X || pos.Y < 0 || m.Length <= pos.Y)
                continue;
            if (!seen.Add((pos.X, pos.Y, direction.X, direction.Y)))
                continue;

            var current = m[pos.Y][pos.X];
            List<Vector2> newDirections = (current) switch
            {
                '.' => [direction],
                '/' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Left()],
                '/' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Right()],
                '\\' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Right()],
                '\\' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Left()],
                '|' when direction.Abs() == new Vector2(0, 1) => [direction],
                '-' when direction.Abs() == new Vector2(1, 0) => [direction],
                '|' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Right(), direction.Rotate90Left()],
                '-' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Right(), direction.Rotate90Left()],
                _ => throw new ArgumentOutOfRangeException()
            };

            foreach (var newDirection in newDirections)
                stack.Push(((pos + newDirection), newDirection));
        }
        return seen.Select(tpl => new Vector2(tpl.Item1, tpl.Item2)).ToHashSet();
    }

    static ISet<Vector2> TraverseRec(char[][] m, Vector2 pos, Vector2 direction)
    {
        var seen = new HashSet<(int, int, int, int)>();
        TraverseRec(m, pos, direction, seen);
        return seen.Select(tpl => new Vector2(tpl.Item1, tpl.Item2)).ToHashSet();
    }

    static void TraverseRec(char[][] m, Vector2 pos, Vector2 direction, ISet<(int, int, int, int)> seen)
    {
        if (pos.X < 0 || m[0].Length <= pos.X || pos.Y < 0 || m.Length <= pos.Y)
            return;
        if (!seen.Add((pos.X, pos.Y, direction.X, direction.Y)))
            return;

        var current = m[pos.Y][pos.X];
        List<Vector2> newDirections = (current) switch
        {
            '.' => [direction],
            '/' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Left()],
            '/' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Right()],
            '\\' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Right()],
            '\\' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Left()],
            '|' when direction.Abs() == new Vector2(0, 1) => [direction],
            '-' when direction.Abs() == new Vector2(1, 0) => [direction],
            '|' when direction.Abs() == new Vector2(1, 0) => [direction.Rotate90Right(), direction.Rotate90Left()],
            '-' when direction.Abs() == new Vector2(0, 1) => [direction.Rotate90Right(), direction.Rotate90Left()],
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var newDirection in newDirections)
        {
            TraverseRec(m, pos + newDirection, newDirection, seen);
        }
    }

    void Print(char[][] m, ISet<Vector2> visited)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                sb.Append(visited.Contains(new Vector2(col, row)) ? '#' : m[row][col]);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
}

readonly struct Vector2(int x, int y)
{
    public readonly int X = x;
    public readonly int Y = y;

    public Vector2 Rotate90Left() => new Vector2(x: Y, y: X * -1);

    public Vector2 Rotate90Right() => new Vector2(x: Y * -1, y: X);

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2 Up => new(0, -1);
    public static Vector2 Down => new(0, 1);
    public static Vector2 Left => new(-1, 0);
    public static Vector2 Right => new(1, 0);

    public Vector2 Abs() => Abs(this);

    public static Vector2 Abs(Vector2 v) => new(Math.Abs(v.X), Math.Abs(v.Y));

    public override bool Equals(object? obj) => obj is Vector2 other && this.Equals(other);

    public bool Equals(Vector2 p) => X == p.X && Y == p.Y;

    public override int GetHashCode() => (X, Y).GetHashCode();

    public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.Equals(rhs);

    public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);
}
