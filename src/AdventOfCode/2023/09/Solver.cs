namespace AdventOfCode.Y2023.D09;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
            .Select(PredictNextValue)
            .Sum()
            .ToString();
    }

    private static int PredictNextValue(int[] history)
    {
        return history[^1] + GetDifferencesForwards(history);
    }

    private static int GetDifferencesForwards(int[] values)
    {
        var differences = new int[values.Length - 1];
        for (var i = 0; i < differences.Length; i++)
        {
            differences[i] = values[i + 1] - values[i];
        }
        if (differences.All(x => x == 0))
            return 0;

        return differences[^1] + GetDifferencesForwards(differences);
    }

    public string PartTwo(string input)
    {
        return input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
            .Select(PredictPreviousValue)
            .Sum()
            .ToString();
    }

    private static int PredictPreviousValue(int[] history)
    {
        return history[0] - GetDifferencesBackwards(history);
    }

    private static int GetDifferencesBackwards(int[] values)
    {
        var differences = new int[values.Length - 1];
        for (var i = 0; i < differences.Length; i++)
        {
            differences[i] = values[i + 1] - values[i];
        }
        if (differences.All(x => x == 0))
            return 0;
        return differences[0] - GetDifferencesBackwards(differences);
    }
}
