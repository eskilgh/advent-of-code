namespace AdventOfCode.Y2022.D01;

internal class Solver : ISolver
{
  public string PartOne(string input)
  {
    return GetCalories(input).Max().ToString();
  }

  public string PartTwo(string input)
  {
    return GetCalories(input).Order().TakeLast(3).Sum().ToString();
  }

  public static IEnumerable<int> GetCalories(string input)
  {
    return input
      .Split("\n\n")
      .Select(
        lines => lines
          .Split("\n")
          .Select(line => Convert.ToInt32(line))
          .Sum()
    );
  }

}
