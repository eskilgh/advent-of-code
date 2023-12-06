namespace AdventOfCode.Y2023.D06;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var lines = input.Split('\n');
        var raceTimes = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..].Select(int.Parse).ToArray();
        var recordedDistances = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..].Select(int.Parse).ToArray();
        var numWaysToWin = new int[raceTimes.Length];
        for (var i = 0; i < raceTimes.Length; i++)
        {
            var recordedDistance = recordedDistances[i];
            var raceTime = raceTimes[i];
            for (var t = 0; t <= raceTimes[i]; t++)
            {
                if (CalcDistance(t, raceTime) > recordedDistance)
                    numWaysToWin[i]++;
            }
        }

        return numWaysToWin.Aggregate((a, b) => a * b).ToString();
    }

    private static int CalcDistance(int time, int maxTime)
    {
        return (maxTime - time) * time;
    }

    public string PartTwo(string input)
    {
        var lines = input.Split('\n');
        var raceTime = long.Parse(string.Join("", lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..]));
        var distance = long.Parse(string.Join("", lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..]));

        // Numerical estimate of analytic second degree polynomial solution
        var t_1 = raceTime / 2 - Math.Sqrt(Math.Pow(raceTime, 2) - 4 * distance) / 2;
        var t_2 = raceTime / 2 + Math.Sqrt(Math.Pow(raceTime, 2) - 4 * distance) / 2;
        return (Math.Ceiling(t_2) - Math.Floor(t_1)).ToString();
    }
}
