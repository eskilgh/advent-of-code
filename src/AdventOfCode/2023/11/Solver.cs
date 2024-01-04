using System.Numerics;
using System.Text;

namespace AdventOfCode.Y2023.D11;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(s => s.ToList()).ToList();
        var emptyRows = m.Select((cs, row) => new { Row = row, Characters = cs })
            .Where(a => a.Characters.All(c => c == '.'))
            .Select(a => a.Row)
            .ToArray();
        var emptyCols = m[0].Select((_, col) => col).Where(col => m.All((cs) => cs[col] == '.')).ToArray();

        var rowsInserted = 0;
        var colsInserted = 0;
        foreach (var row in emptyRows)
            m.Insert(row + rowsInserted++, m[row].Select(_ => '.').ToList());
        foreach (var col in emptyCols)
        {
            for (var r = 0; r < m.Count; r++)
                m[r].Insert(col + colsInserted, '.');
            colsInserted++;
        }

        var galaxies = (
            from row in m.Select((_, i) => i)
            from col in m[0].Select((_, j) => j)
            where m[row][col] == '#'
            select new Point(row, col)
        );

        var combinations =
            from p1 in galaxies
            from p2 in galaxies
            where (p1.Row < p2.Row) || (p1.Row == p2.Row && p1.Col < p2.Col)
            select new { p1, p2 };

        return combinations.Sum(ps => Distance(ps.p1, ps.p2));
    }

    private static int Distance(Point a, Point b) => Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);

    private struct Point(int row, int col)
    {
        public int Row = row;
        public int Col = col;
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(s => s.ToList()).ToList();
        var emptyRows = m.Select((cs, row) => new { Row = row, Characters = cs })
            .Where(a => a.Characters.All(c => c == '.'))
            .Select(a => a.Row)
            .ToArray();
        var emptyCols = m[0].Select((_, col) => col).Where(col => m.All((cs) => cs[col] == '.')).ToArray();

        var galaxies = (
            from row in m.Select((_, i) => i)
            from col in m[0].Select((_, j) => j)
            where m[row][col] == '#'
            select new Point(row, col)
        );

        var combinations =
            from p1 in galaxies
            from p2 in galaxies
            where (p1.Row < p2.Row) || (p1.Row == p2.Row && p1.Col < p2.Col)
            select new { p1, p2 };

        return combinations.Aggregate(BigInteger.Zero, (acc, ps) => acc + Distance(ps.p1, ps.p2, emptyRows, emptyCols));
    }

    private static BigInteger Distance(Point a, Point b, int[] emptyRows, int[] emptyCols)
    {
        var expansionMultiplier = new BigInteger(1000000);
        var rowMin = Math.Min(a.Row, b.Row);
        var rowMax = Math.Max(a.Row, b.Row);
        var colMin = Math.Min(a.Col, b.Col);
        var colMax = Math.Max(a.Col, b.Col);
        var crossedEmptyRows = emptyRows.Where(row => rowMin < row && row < rowMax).Count();
        var crossedEmptyCols = emptyCols.Where(col => colMin < col && col < colMax).Count();
        var ans =
            rowMax
            - rowMin
            + colMax
            - colMin
            - crossedEmptyRows
            - crossedEmptyCols
            + (expansionMultiplier * crossedEmptyRows)
            + (expansionMultiplier * crossedEmptyCols);
        return ans;
    }

    private static void Print(IEnumerable<IEnumerable<char>> m)
    {
        var sb = new StringBuilder();
        foreach (var r in m)
        {
            foreach (var c in r)
                sb.Append(c);
            sb.Append('\n');
        }
        Console.WriteLine(sb.ToString());
    }
}
