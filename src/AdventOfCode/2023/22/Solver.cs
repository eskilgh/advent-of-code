using AdventOfCode.Common;
using System.Drawing;
using System.Text;

namespace AdventOfCode.Y2023.D22;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var bricks = input.Split('\n').Select((line, i) =>
        {
            char name = (char)('A' + i);
            var coords = line.Split('~').Select(part => part.Split(',').Select(int.Parse).ToArray()).ToArray();
            var p1 = (x: coords[0][0], y: coords[0][1], z: coords[0][2]);
            var p2 = (x: coords[1][0], y: coords[1][1], z: coords[1][2]);
            return (name, p1, p2);
        }).ToArray();

        Print(bricks);
        return "Not available";
    }




    public object PartTwo(string input)
    {
        return "Not available";
    }
    private void Print((char name, (int x, int y, int z) p1, (int x, int y, int z) p2)[] bricks)
    {
        var xMin = bricks.Select(brick => Math.Min(brick.p1.x, brick.p2.x)).Min();
        var xMax = bricks.Select(brick => Math.Max(brick.p1.x, brick.p2.x)).Max();
        var zMin = 0;
        var zMax = bricks.Select(brick => Math.Max(brick.p1.z, brick.p2.z)).Max();

        var height = zMax - zMin + 1;
        var width = xMax - xMin + 1;
        var m = Enumerable.Range(0, height).Select(_ => Enumerable.Repeat('.', width).ToArray()).ToArray();
        

        foreach (var brick in bricks)
        {
            for (var z = Math.Min(brick.p1.z, brick.p2.z); z <= Math.Max(brick.p1.z, brick.p2.z); z++)
            {
                var row = height - z - 1;
                for (var col = Math.Min(brick.p1.x, brick.p2.x); col <= Math.Max(brick.p1.x, brick.p2.x); col++)
                {
                    m[row][col] = brick.name;
                }
            }
        }
        m.Print();
    }

    
}
