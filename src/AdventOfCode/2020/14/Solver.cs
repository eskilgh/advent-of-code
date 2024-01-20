using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2020.D14;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var vals = new Dictionary<long, long>();
        var mask = string.Empty;
        foreach (var line in input.Split('\n'))
        {
            var parts = line.Split('=', StringSplitOptions.TrimEntries);
            if (parts[0] is "mask")
                mask = parts[1];
            else
            {
                var address = long.Parse(Regex.Match(parts[0], @"-?\d+(\.\d+)?").Value);
                var value = long.Parse(parts[1]);
                vals[address] = ApplyValueMask(value, mask);
            }
        }

        return vals.Values.Sum();
    }

    public object PartTwo(string input)
    {
        var vals = new Dictionary<string, long>();
        var mask = string.Empty;
        foreach (var line in input.Split('\n'))
        {
            var parts = line.Split('=', StringSplitOptions.TrimEntries);
            if (parts[0] is "mask")
                mask = parts[1];
            else
            {
                var address = long.Parse(Regex.Match(parts[0], @"-?\d+(\.\d+)?").Value);
                var addresses = ApplyAddressMask(address, mask);
                var value = long.Parse(parts[1]);
                foreach (var a in addresses)
                {
                    vals[a] = value;
                }
            }
        }

        return vals.Values.Sum();
    }

    static IEnumerable<string> ApplyAddressMask(long address, string mask)
    {
        var addressBin = Convert.ToString(address, 2).PadLeft(mask.Length, '0');
        return ApplyAddressMask(addressBin, mask, 0);
    }

    static IEnumerable<string> ApplyAddressMask(string address, string mask, int i)
    {
        if (i == mask.Length - 1)
        {
            return mask[i] switch
            {
                '0' => [address[i].ToString()],
                '1' => ["1"],
                'X' => ["0", "1"],
                _ => throw new ArgumentException()
            };
        }
        var rest = ApplyAddressMask(address, mask, i + 1);
        return mask[i] switch
        {
            '0' => rest.Select(s => address[i] + s),
            '1' => rest.Select(s => '1' + s),
            'X'
                => rest.Select(s => '0' + s).Concat(rest.Select(s => '1' + s)),
            _ => throw new ArgumentException()
        };
    }

    static long ApplyValueMask(long value, string mask)
    {
        var valueBin = Convert.ToString(value, 2).PadLeft(mask.Length, '0');
        var app = mask.Zip(valueBin)
            .Select(
                tpl =>
                    tpl switch
                    {
                        ('X', char c) => c,
                        ('1', _) => '1',
                        ('0', _) => '0',
                        _ => throw new ArgumentException()
                    }
            )
            .ToArray();
        return Convert.ToInt64(new string(app), 2);
    }
}
