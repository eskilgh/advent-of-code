namespace AdventOfCode.Common;

/// <summary>
/// An inclusive Interval
/// </summary>
public record Interval(double Min, double Max)
{
    public bool Contains(double value) => Min <= value && value <= Max;
};
