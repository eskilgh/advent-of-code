namespace AdventOfCode.Verbs.Scaffold;

internal class SolverTemplateGenerator
{
  internal static string Generate(string year, string day)
  {
    return $$"""
    namespace AdventOfCode.Y{{year}}.D{{day}};

    internal class Solver : ISolver
    {
      public string PartOne(string input)
      {
        return "Not available";
      }

      public string PartTwo(string input)
      {
        return "Not available";
      }
    }

    """;
  }
}