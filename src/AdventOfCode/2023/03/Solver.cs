using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.D03;

internal class Solver : ISolver
{
  public string PartOne(string input)
  {
    var lines = input
      .Split('\n');
    var nums = ParseNumbers(lines);

    return nums.Where(num =>
      IsPartNumber(num.Row, num.Col, num.Length, lines))
      .Sum(num => num.Value)
      .ToString();
  }

  private static IEnumerable<Number> ParseNumbers(string[] lines)
  {
    const string numberPattern = @"\b\d+\b";
    return lines
      .SelectMany((line, i) =>
        Regex.Matches(line, numberPattern)
        .Select(
      match => new Number
      {
        Value = int.Parse(match.Value),
        Row = i,
        Col = match.Index,
        Length = match.Length
      }));
  }

  private static bool IsPartNumber(int rowPos, int colPos, int length, string[] lines)
  {
    static bool IsSymbol(char c) => c != '.' && !char.IsDigit(c);
    var startCol = Math.Max(colPos - 1, 0);
    var endCol = Math.Min(colPos + length, lines[0].Length - 1);

    if (rowPos > 0)
    {
      for (var col = startCol; col <= endCol; col++)
      {
        if (IsSymbol(lines[rowPos - 1][col])) return true;
      }
    }
    if (colPos > 0)
    {
      if (IsSymbol(lines[rowPos][colPos - 1])) return true;
    }
    if (colPos + length < lines[0].Length)
    {
      if (IsSymbol(lines[rowPos][colPos + length])) return true;
    }
    if (rowPos < lines.Length - 1)
    {
      for (var col = startCol; col <= endCol; col++)
      {
        if (IsSymbol(lines[rowPos + 1][col])) return true;
      }
    }
    return false;
  }


  public string PartTwo(string input)
  {
    var lines = input.Split('\n');
    var nums = ParseNumbers(lines);

    return lines.SelectMany((line, row) =>
      line
        .Select((c, col) =>
        {
          if (c != '*')
            return 0;

          var adjacentNumbers =
            nums
            .Where(num =>
            {
              return Math.Abs(row - num.Row) <= 1 &&
              num.Col - 1 <= col && col <= num.Col + num.Length;
            })
            .ToList();

          return adjacentNumbers.Count == 2
            ? adjacentNumbers[0].Value * adjacentNumbers[1].Value
            : 0;
        })
      )
      .Sum()
      .ToString();
  }

  private struct Number
  {
    public int Value;
    public int Row;
    public int Col;
    public int Length;
  }
}
