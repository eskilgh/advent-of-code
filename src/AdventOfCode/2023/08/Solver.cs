using System.Numerics;
using AngleSharp.Dom;

namespace AdventOfCode.Y2023.D08;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var instructions = lines[0];

        var nodes = lines[1..]
            .Select(line =>
            {
                return new Node
                {
                    Value = line.Split(' ')[0],
                    Left = string.Join("", line[7..10]),
                    Right = string.Join("", line[12..15])
                };
            })
            .ToDictionary(node => node.Value, node => node);

        var current = nodes["AAA"];
        const string target = "ZZZ";

        var steps = 0;
        while (current.Value != target)
        {
            current = instructions[steps % instructions.Length] switch
            {
                'L' => nodes[current.Left],
                'R' => nodes[current.Right],
                _ => throw new ArgumentOutOfRangeException()
            };
            steps++;
        }

        return steps;
    }

    public object PartTwo(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var instructions = lines[0];

        var nodes = lines[1..]
            .Select(line =>
            {
                return new Node
                {
                    Value = line.Split(' ')[0],
                    Left = string.Join("", line[7..10]),
                    Right = string.Join("", line[12..15])
                };
            })
            .ToDictionary(node => node.Value, node => node);

        return nodes
            .Values
            .Where(node => node.Value[2] == 'A')
            .Select(node =>
            {
                var currentNode = node;
                var steps = 0;
                while (currentNode.Value[2] != 'Z')
                {
                    currentNode = instructions[steps % instructions.Length] switch
                    {
                        'L' => nodes[currentNode.Left],
                        'R' => nodes[currentNode.Right],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    steps++;
                }
                return steps;
            })
            .Select(Convert.ToInt64)
            .Aggregate(LCM);
    }

    static long LCM(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }

    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}

public class Node
{
    public string Value = string.Empty;
    public string Left = string.Empty;
    public string Right = string.Empty;
}
