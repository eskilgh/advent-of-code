using AngleSharp.Text;

namespace AdventOfCode.Y2023.D01;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var answer = SumOfDigitsPartOne(input.Split('\n'));
        return answer.ToString();
    }

    private int SumOfDigitsPartOne(string[] lines)
    {
        return lines
            .Select(line =>
            {
                var l = 0;
                var r = line.Length - 1;
                while (!char.IsDigit(line[l]))
                    l++;
                while (!char.IsDigit(line[r]))
                    r--;
                return (line[l] - '0') * 10 + (line[r] - '0');
            })
            .Sum();
    }

    public string PartTwo(string input)
    {
        return SumOfDigitsPartTwo(input.Split('\n')).ToString();
    }

    private int SumOfDigitsPartTwo(string[] lines)
    {
        return lines
            .Select(line =>
            {
                var l = 0;
                var r = line.Length - 1;

                var leftDigit = -1;
                var rightDigit = -1;
                while (leftDigit < 0)
                {
                    leftDigit = GetDigit(line, l, Direction.Right);
                    l++;
                }
                while (rightDigit < 0)
                {
                    rightDigit = GetDigit(line, r, Direction.Left);
                    r--;
                }
                return leftDigit * 10 + rightDigit;
            })
            .Sum();
    }

    private int GetDigit(string line, int index, Direction direction)
    {
        if (char.IsDigit(line[index]))
            return line[index] - '0';

        // match on spelled out
        Dictionary<string, int> threeLetters =
            new()
            {
                { "one", 1 },
                { "two", 2 },
                { "six", 6 },
            };
        Dictionary<string, int> fourLetters =
            new()
            {
                { "four", 4 },
                { "five", 5 },
                { "nine", 9 }
            };
        Dictionary<string, int> fiveLetters =
            new()
            {
                { "three", 3 },
                { "seven", 7 },
                { "eight", 8 }
            };

        if (
            threeLetters.TryGetValue(
                Substring(line, index, 3, direction),
                out var threeLetterNumber
            )
        )
            return threeLetterNumber;
        if (fourLetters.TryGetValue(Substring(line, index, 4, direction), out var fourLetterNumber))
            return fourLetterNumber;
        if (fiveLetters.TryGetValue(Substring(line, index, 5, direction), out var fiveLetterNumber))
            return fiveLetterNumber;

        return -1;
    }

    private string Substring(string line, int index, int length, Direction direction)
    {        
        return direction switch
        {
            Direction.Right => line.Substring(index, Math.Min(length, line.Length - index)),
            Direction.Left => line.Substring(Math.Max(index - length + 1, 0), Math.Min(length, line.Length)),
        };
    }

    private enum Direction
    {
        Right,
        Left
    }
}
