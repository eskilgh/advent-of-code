namespace AdventOfCode.Verbs.Scaffold;

internal class SolverTemplateGenerator
{
    internal static string Generate(string year, string day) => $$"""
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

    internal static string GenerateFSharp(string year, string day) => $"""
        module Year{year}Day{day}

        type Solver() =
            interface Shared.ISolver with
                member this.PartOne(input: string) : obj = "Not available"

                member this.PartTwo(input: string) : obj = "Not available"

        """;
}
