namespace AdventOfCode.Y2022.D08;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var matrix = ParseMatrix(input);
        return CountVisibleTrees(matrix);
    }

    public object PartTwo(string input)
    {
        var matrix = ParseMatrix(input);
        return GetMaxScenicScore(matrix);
    }

    private static int GetMaxScenicScore(int[][] matrix)
    {
        var maxScenicScore = -1;
        for (var row = 0; row < matrix.Length; row++)
        for (var col = 0; col < matrix.Length; col++)
            maxScenicScore = Math.Max(maxScenicScore, GetScenicScore(matrix, row: row, col: col));

        return maxScenicScore;
    }

    private static int GetScenicScore(int[][] matrix, int row, int col)
    {
        var directions = (Direction[])Enum.GetValues(typeof(Direction));
        return directions
            .Select(direction => GetScenicScoreForDirection(matrix, row, col, direction))
            .Aggregate((acc, e) => acc * e);
    }

    private static int GetScenicScoreForDirection(int[][] matrix, int row, int col, Direction direction)
    {
        var (rowInc, colInc) = direction.ToTuple();
        var scenicScore = 0;
        var value = matrix[row][col];
        var r = row + rowInc;
        var c = col + colInc;
        while (0 <= r && r < matrix.Length && 0 <= c && c < matrix[0].Length)
        {
            scenicScore++;
            if (matrix[r][c] >= value)
                return scenicScore;
            r += rowInc;
            c += colInc;
        }
        return scenicScore;
    }

    private static int CountVisibleTrees(int[][] matrix)
    {
        var visibleTrees = (matrix.Length * 2) + (matrix[0].Length - 2) * 2;
        for (var row = 1; row < matrix.Length - 1; row++)
        for (var col = 1; col < matrix.Length - 1; col++)
        {
            if (IsVisible(matrix, row: row, col: col))
                visibleTrees++;
        }
        return visibleTrees;
    }

    private static bool IsVisible(int[][] matrix, int row, int col)
    {
        var directions = (Direction[])Enum.GetValues(typeof(Direction));
        return directions.Any(direction => IsVisibleFromDirection(matrix, row, col, direction));
    }

    private static bool IsVisibleFromDirection(int[][] matrix, int row, int col, Direction direction)
    {
        var (rowInc, colInc) = direction.ToTuple();

        var value = matrix[row][col];
        var r = row + rowInc;
        var c = col + colInc;
        while (0 <= r && r < matrix.Length && 0 <= c && c < matrix[0].Length)
        {
            if (matrix[r][c] >= value)
                return false;
            r += rowInc;
            c += colInc;
        }
        return true;
    }

    private static int[][] ParseMatrix(string input)
    {
        return input.Split("\n").Select(line => line.Select(c => (int)(c - '0')).ToArray()).ToArray();
    }
}

internal enum Direction
{
    Up,
    Left,
    Down,
    Right,
}

file static class Extensions
{
    public static (int RowInc, int ColInc) ToTuple(this Direction direction) =>
        direction switch
        {
            Direction.Up => (-1, 0),
            Direction.Left => (0, -1),
            Direction.Down => (1, 0),
            Direction.Right => (0, 1),
            _ => throw new ArgumentException("")
        };
}
