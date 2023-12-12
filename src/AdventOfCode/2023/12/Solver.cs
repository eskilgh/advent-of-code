namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var lines = input.Split('\n');
        var conditions = lines.Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]);
        var groupings = lines.Select(
            line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Split(',').Select(int.Parse)
        );

        return "Not available";
    }

    static string FindPermutations(char[] cs, int[] groups)
    {
        var i = 0;
        var possiblePermutations = 0;


        var prev = '.';
        var consecutiveCount = 0;

        FindPermutationsRec(cs, groups, 0);

        while (i < cs.Length)
        {
            if ()
        }
    }

    static int FindPermutationsRec(char[] cs, int[] groups, int i) 
    {
        var target = groups.FirstOrDefault(group => group > 0);
        if (target == 0)
            return 1;

        
        var prev = i == 0 ? '.' : cs[i - 1];


    }


    public string PartTwo(string input)
    {
        return "Not available";
    }
}
