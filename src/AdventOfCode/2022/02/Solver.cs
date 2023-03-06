namespace AdventOfCode.Y2022.D02;

internal class Solver : ISolver
{
  public string PartOne(string input)
  {
    return CalculateScorePartOne(input).ToString();
  }

  public string PartTwo(string input)
  {
    return CalculateScorePartTwo(input).ToString();
  }

  private static int CalculateScorePartOne(string input)
  {
    return input
      .Split("\n")
      .Select(line => line.ToCharArray())
      .Select(cs => CalculateRoundScore(cs[0] - 'A', cs[^1] - 'X'))
      .Sum();
  }

  private static int CalculateScorePartTwo(string input)
  {
    return input
      .Split("\n")
      .Select(line => line.ToCharArray())
      .Select(cs => ChooseCorrectResponse(cs[0] - 'A', cs[^1] - 'X'))
      .Select(tpl => CalculateRoundScore(tpl.Item1, tpl.Item2))
      .Sum();
  }

  private static (int, int) ChooseCorrectResponse(int leftNormalized, int rightNormalized)
  {
    var newNormalizedRight = rightNormalized switch
    {
      0 => Modulo(leftNormalized - 1, 3), // lose
      1 => leftNormalized, // draw
      2 => Modulo(leftNormalized + 1, 3),
      _ => throw new ArgumentException("")
    };
    return (leftNormalized, newNormalizedRight);
  }

  private static int CalculateRoundScore(int leftNormalized, int rightNormalized)
  {
    var outcomeScore = Modulo(leftNormalized - rightNormalized, 3) switch
    {
      0 => 3, // draw
      1 => 0, // loss
      2 => 6, // win
      _ => throw new ArgumentException("")
    };
    var shapeScore = rightNormalized + 1;
    return outcomeScore + shapeScore;
  }

  // '%' operator returns negative numbers, this is a mathematical modulo
  private static int Modulo(int a, int b) => (Math.Abs(a * b) + a) % b;
}
