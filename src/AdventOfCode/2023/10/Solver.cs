using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.D10;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        var (row, col) = IndexOf2d(m, 'S');
        var direction = FindStartingDirection(m, row, col);
        var current = m[row][col];
        var length = 0;
        while (current != 'S' || length == 0)
        {
            (row, col) = direction switch
            {
                'N' => (row - 1, col),
                'E' => (row, col + 1),
                'S' => (row + 1, col),
                'W' => (row, col - 1),
                _ => throw new ArgumentException()
            };
            current = m[row][col];
            direction = NextDirection(current, direction);
            length++;
        }
        return (length / 2).ToString();
    }

    public string PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        var (row, col) = IndexOf2d(m, 'S');
        var direction = FindStartingDirection(m, row, col);
        var current = m[row][col];
        var seen = new HashSet<(int Row, int Col)>();
        while (current != 'S' || seen.Count == 0)
        {
            seen.Add((row, col));
            (row, col) = direction switch
            {
                'N' => (row - 1, col),
                'E' => (row, col + 1),
                'S' => (row + 1, col),
                'W' => (row, col - 1),
                _ => throw new ArgumentException()
            };
            current = m[row][col];
            direction = NextDirection(current, direction);
        }

        return CalculateEnclosedArea(m, seen).ToString();
    }

    private int CalculateEnclosedArea(char[][] m, HashSet<(int Row, int Col)> seen)
    {
        var rowMin = seen.MinBy(tpl => tpl.Row).Row;
        var rowMax = seen.MaxBy(tpl => tpl.Row).Row;
        var colMin = seen.MinBy(tpl => tpl.Col).Col;
        var colMax = seen.MaxBy(tpl => tpl.Col).Col;

        var count = 0;
        for (var row = rowMin; row <= rowMax; row++)
        {
            var crossCount = 0;
            for (var col = colMin; col <= colMax; col++)
            {
                var isPointOnLoop = seen.Contains((row, col));
                var isEnclosedPoint = !isPointOnLoop && crossCount % 2 == 1;
                var current = m[row][col];
                if (current is 'S')
                    current = GetUnderlyingPipeOfStart(m, row, col);
                if (isPointOnLoop && current is '|' or 'J' or 'L')
                    crossCount++;
                if (isEnclosedPoint)
                    count++;
            }
        }
        return count;
    }

    

    private static char FindStartingDirection(char[][] m, int row, int col)
    {
        if (NextDirection(m[row - 1][col], 'N') != '.')
            return 'N';
        if (NextDirection(m[row][col + 1], 'E') != '.')
            return 'E';
        if (NextDirection(m[row][col - 1], 'W') != '.')
            return 'W';
        if (NextDirection(m[row + 1][col], 'S') != '.')
            return 'S';

        throw new ArgumentOutOfRangeException();
    }

    private static char GetUnderlyingPipeOfStart(char[][] m, int row, int col)
    {
        if (m[row][col] != 'S')
            throw new ArgumentException();

        var sb = new StringBuilder();

        if (NextDirection(m[row - 1][col], 'N') != '.')
            sb.Append('N');
        if (NextDirection(m[row][col + 1], 'E') != '.')
            sb.Append('E');
        if (NextDirection(m[row + 1][col], 'S') != '.')
            sb.Append('S');
        if (NextDirection(m[row][col - 1], 'W') != '.')
            sb.Append('W');

        return sb.ToString() switch
        {
            "NS" => '|',
            "EW" => '-',
            "NE" => 'L',
            "NW" => 'J',
            "SW" => '7',
            "SE" => 'F',
            _ => throw new ArgumentException()
        };
    }

    private static (int Row, int Col) IndexOf2d<T>(T[][] m, T value)
    {
        for (var row = 0; row < m.Length; row++)
        {
            var col = Array.IndexOf(m[row], value);
            if (col != -1)
                return (row, col);
        }
        return (-1, -1);
    }

    private static char NextDirection(char pipe, char currentDirection)
    {
        var pipeDirections = PipeDirections(pipe);
        if (!pipeDirections.Remove(Opposite(currentDirection)))
            return '.';
        return pipeDirections.Single();
    }

    private static HashSet<char> PipeDirections(char c)
    {
        return c switch
        {
            '|' => new() { 'N', 'S' },
            '-' => new() { 'E', 'W' },
            'L' => new() { 'N', 'E' },
            'J' => new() { 'N', 'W' },
            '7' => new() { 'S', 'W' },
            'F' => new() { 'S', 'E' },
            _ => new()
        };
    }

    private static char Opposite(char c)
    {
        return c switch
        {
            'N' => 'S',
            'S' => 'N',
            'E' => 'W',
            'W' => 'E',
            _ => throw new ArgumentOutOfRangeException(nameof(c))
        };
    }
}
