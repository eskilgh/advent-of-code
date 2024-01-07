namespace AdventOfCode.Y2020.D03;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        return TreesInPath(m, 1, 3);
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        return new (int colInc, int rowInc)[]
        {
            (1, 1),
            (3, 1),
            (5, 1),
            (7, 1),
            (1, 2)
        }.Select(tpl => TreesInPath(m, tpl.rowInc, tpl.colInc)).Aggregate((long) 1, (a, b) => a * b);
    }

    private static int TreesInPath(char[][] m, int rowInc, int colInc)
    {
        var (row, col) = (rowInc, colInc);
        var width = m[0].Length;
        var trees = 0;
        while (row < m.Length)
        {
            var current = m[row][col % width];
            if (current is '#')
                trees++;
            (row, col) = (row + rowInc, col + colInc);
        }
        return trees;
    }
}
