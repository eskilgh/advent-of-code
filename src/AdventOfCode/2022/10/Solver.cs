using System.Text;

namespace AdventOfCode.Y2022.D10;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var lines = input.Split("\n").ToArray();
        var value = 1;
        var numbersToAdd = new Queue<int>();
        var cycle = 1;
        var sumSignalStrengths = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            while (numbersToAdd.TryDequeue(out var val))
            {
                if (Qualifes(cycle))
                {
                    sumSignalStrengths += cycle * value;
                }
                value += val;
                cycle++;
            }
            var instruction = line.Split(" ")[0];
            numbersToAdd.Enqueue(0);
            if (instruction != "noop")
                numbersToAdd.Enqueue(int.Parse(line.Split(" ")[1]));
        }
        while (numbersToAdd.TryDequeue(out var val))
        {
            if (Qualifes(cycle))
            {
                sumSignalStrengths += cycle * value;
            }
            cycle++;
            value += val;
        }
        return sumSignalStrengths;
    }

    private static bool Qualifes(int cycle) => cycle == 20 || cycle > 20 && (cycle - 20) % 40 == 0;

    public object PartTwo(string input)
    {
        var width = 40;
        var sb = new StringBuilder();
        var lines = input.Split("\n").ToArray();
        var sprite = 1;
        var numbersToAdd = new Queue<int>();
        var cycle = 1;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            while (numbersToAdd.TryDequeue(out var val))
            {
                var crtCol = (cycle - 1) % width;
                var pixel = sprite - 1 <= crtCol && crtCol <= sprite + 1 ? '#' : '.';
                sb.Append(pixel);
                if (crtCol == width - 1)
                {
                    sb.Append('\n');
                }
                sprite += val;
                cycle++;
            }
            var instruction = line.Split(" ")[0];
            numbersToAdd.Enqueue(0);
            if (instruction != "noop")
                numbersToAdd.Enqueue(int.Parse(line.Split(" ")[1]));
        }
        while (numbersToAdd.TryDequeue(out var val))
        {
            var crtCol = (cycle - 1) % width;
            var pixel = sprite - 1 <= crtCol && crtCol <= sprite + 1 ? '#' : '.';
            sb.Append(pixel);
            if (crtCol == width - 1)
            {
                sb.Append('\n');
            }
            cycle++;
            sprite += val;
        }

        return sb.ToString();
    }
}
