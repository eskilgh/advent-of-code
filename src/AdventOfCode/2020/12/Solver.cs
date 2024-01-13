using MathNet.Numerics;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Y2020.D12;

public static class Directions
{
    public static Complex North => -Complex.ImaginaryOne;
    public static Complex South => Complex.ImaginaryOne;
    public static Complex East => Complex.One;
    public static Complex West => -Complex.One;
}

file static class Extensions
{
    public static Complex Rotate(this Complex z, int angleInDegrees)
    {
        var radians = Math.PI * angleInDegrees / 180;
        var rotationFactor = new Complex(Math.Round(Math.Cos(radians)), Math.Round(Math.Sin(radians)));
        var ans = z * rotationFactor;

        return z * rotationFactor;
    }
}

internal class Solver : ISolver
{
    public void RotateTest()
    {
        var a = Directions.North.Rotate(90) == Directions.East;
        var b = Directions.North.Rotate(180) == Directions.South;
        var c = Directions.North.Rotate(270) == Directions.West;
        var d = Directions.North.Rotate(360) == Directions.North;

        var e = Directions.North.Rotate(-90) == Directions.West;
        var f = Directions.North.Rotate(-180) == Directions.South;
        var g = Directions.North.Rotate(-270) == Directions.East;
        var h = Directions.North.Rotate(-360) == Directions.North;
        Console.WriteLine($"{a}{b}{c}{d}{e}{f}{g}{h}");
    }

    public void RotateTest2()
    {
        var a = Directions.East.Rotate(90) == Directions.South;
        var b = Directions.East.Rotate(180) == Directions.West;
        var c = Directions.East.Rotate(270) == Directions.North;
        var d = Directions.East.Rotate(360) == Directions.East;

        var e = Directions.East.Rotate(-90) == Directions.North;
        var f = Directions.East.Rotate(-180) == Directions.West;
        var g = Directions.East.Rotate(-270) == Directions.South;
        var h = Directions.East.Rotate(-360) == Directions.East;
        Console.WriteLine($"{a}{b}{c}{d}{e}{f}{g}{h}");
    }

    public object PartOne(string input)
    {
        var finalPos = input
            .Split('\n')
            .Select(line =>
            {
                var c = line[0];
                var value = int.Parse(line[1..]);
                return (c, value);
            })
            .Aggregate(
                (pos: new Complex(0, 0), dir: Directions.East),
                (current, instruction) =>
                    instruction.c switch
                    {
                        'N' => (current.pos + Directions.North * instruction.value, current.dir),
                        'S' => (current.pos + Directions.South * instruction.value, current.dir),
                        'E' => (current.pos + Directions.East * instruction.value, current.dir),
                        'W' => (current.pos + Directions.West * instruction.value, current.dir),
                        'F' => (current.pos + current.dir * instruction.value, current.dir),
                        'R' => (current.pos, current.dir.Rotate(instruction.value)),
                        'L' => (current.pos, current.dir.Rotate(instruction.value * -1)),
                        _ => throw new ArgumentOutOfRangeException()
                    }
            )
            .pos;
        return Math.Abs(finalPos.Real) + Math.Abs(finalPos.Imaginary);
    }

    public object PartTwo(string input)
    {
        var finalPos = input
           .Split('\n')
           .Select(line =>
           {
               var c = line[0];
               var value = int.Parse(line[1..]);
               return (c, value);
           })
           .Aggregate(
               (ship: new Complex(0, 0), waypoint: new Complex(10, -1)),
               (current, instruction) =>
                    instruction.c switch
                    {
                        'N' => (current.ship, current.waypoint + Directions.North * instruction.value),
                        'S' => (current.ship, current.waypoint + Directions.South * instruction.value),
                        'E' => (current.ship, current.waypoint + Directions.East * instruction.value),
                        'W' => (current.ship, current.waypoint + Directions.West * instruction.value),
                        'F' => (current.ship + current.waypoint * instruction.value, current.waypoint),
                        'R' => (current.ship, current.waypoint.Rotate(instruction.value)),
                        'L' => (current.ship, current.waypoint.Rotate(instruction.value * -1)),
                        _ => throw new ArgumentOutOfRangeException()
                    }
           )
           .ship;
        return Math.Abs(finalPos.Real) + Math.Abs(finalPos.Imaginary);
    }
}
