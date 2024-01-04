namespace AdventOfCode.Y2022.D01;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return GetCalories(input).Max();
    }

    public object PartTwo(string input)
    {
        return GetCalories(input).Order().TakeLast(3).Sum();
    }

    public static IEnumerable<int> GetCalories(string input)
    {
        return input.Split("\n\n").Select(lines => lines.Split("\n").Select(line => Convert.ToInt32(line)).Sum());
    }
}
