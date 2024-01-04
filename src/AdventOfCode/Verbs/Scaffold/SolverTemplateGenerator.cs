namespace AdventOfCode.Verbs.Scaffold;

internal class SolverTemplateGenerator
{
    internal static string Generate(string year, string day)
    {
        return $$"""
    namespace AdventOfCode.Y{{year}}.D{{day}};

    internal class Solver : ISolver
    {
        public object PartOne(string input)
        {
            return "Not available";
        }

        public object PartTwo(string input)
        {
            return "Not available";
        }
    }

    """;
    }
}
