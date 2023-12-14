using System.Text;

namespace AdventOfCode;

public static class Utils
{
    public static void Print(this IEnumerable<IEnumerable<char>> m)
    {
        var sb = new StringBuilder();
        foreach (var r in m)
        {
            foreach (var c in r)
                sb.Append(c);
            sb.Append('\n');
        }
        Console.WriteLine(sb.ToString());
    }
}
