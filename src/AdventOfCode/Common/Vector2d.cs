namespace AdventOfCode.Common;

public readonly struct Vector2d(int x, int y)
{
    public readonly int X = x;
    public readonly int Y = y;

    public Vector2d Rotate90Left() => new Vector2d(x: Y, y: X * -1);

    public Vector2d Rotate90Right() => new Vector2d(x: Y * -1, y: X);

    public Vector2d Rotate180() => new(x: X * (-1), y: Y * -1);

    public static Vector2d operator +(Vector2d a, Vector2d b) => new(a.X + b.X, a.Y + b.Y);

    public static Vector2d operator -(Vector2d a, Vector2d b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2d Up => new(0, -1);
    public static Vector2d Down => new(0, 1);
    public static Vector2d Left => new(-1, 0);
    public static Vector2d Right => new(1, 0);

    public Vector2d Abs() => Abs(this);

    public static Vector2d Abs(Vector2d v) => new(Math.Abs(v.X), Math.Abs(v.Y));

    public static double SqEuclidianDistance(Vector2d a, Vector2d b) => Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2);

    public static double EuclidianDistance(Vector2d a, Vector2d b) => Math.Sqrt(SqEuclidianDistance(a, b));

    public static int ManhattanDistance(Vector2d a, Vector2d b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    public int ManhattanDistance(Vector2d other) => ManhattanDistance(this, other);

    public bool IsOutOfBounds<T>(T[][] m) => IsOutOfBounds(0, m[0].Length - 1, 0, m.Length - 1);

    /// <returns>True if X and Y fall within the inclusive ranges. False otherwise</returns>
    public bool IsOutOfBounds(int xMin, int xMax, int yMin, int yMax) =>
        !(xMin <= X && X <= xMax && yMin <= Y && Y <= yMax);

    public override bool Equals(object? obj) => obj is Vector2d other && Equals(other);

    public bool Equals(Vector2d p) => X == p.X && Y == p.Y;

    public override int GetHashCode() => (X, Y).GetHashCode();

    public Vector2d ScalarMultiply(int scalar) => new Vector2d(X * scalar, Y * scalar);

    public static bool operator ==(Vector2d lhs, Vector2d rhs) => lhs.Equals(rhs);

    public static bool operator !=(Vector2d lhs, Vector2d rhs) => !(lhs == rhs);
}
