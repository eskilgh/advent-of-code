namespace AdventOfCode.Y2023.D15;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return input.Split(',').Select(Hash).Sum().ToString();
    }

    public string PartTwo(string input)
    {
        var boxes = Enumerable.Range(0, 256).Select(_ => new List<string>()).ToArray();
        var instructions = input
            .Split(',')
            .Select(step =>
            {
                return step switch
                {
                    [.. var label, '-'] => (label, Hash(label), '-', -1),
                    [.. var label, '=', var num] => (label, Hash(label), '=', (num - '0')),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        foreach (var (label, hash, operation, focalLength) in instructions)
        {
            var box = boxes[hash];
            if (operation is '=')
            {
                var newValue = $"{label} {focalLength}";
                var i = box.FindIndex(s => s.StartsWith(label));
                if (i > -1)
                {
                    box[i] = newValue;
                }
                else
                {
                    box.Add(newValue);
                }
            }
            else if (operation is '-')
            {
                var i = box.FindIndex(s => s.StartsWith(label));
                if (i >= 0)
                    box.RemoveAt(i);
            }
        }

        return boxes.Select((box, i) => (1 + i) * box.Select((s, j) => (j + 1) * (s[^1] - '0')).Sum()).Sum().ToString();
    }

    int Hash(IEnumerable<char> cs)
    {
        return cs.Aggregate(0, (currentValue, c) => ((currentValue + c) * 17) % 256);
    }
}
