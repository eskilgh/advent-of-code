using System;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Verbs.Run;

public class Runner
{
    public readonly string Year;
    public readonly string Day;
    private readonly string _rootDir;

    private string InputFilePath => Path.Combine(_rootDir, Year, Day, "input.txt");
    private string ExampleInputFilePath => Path.Combine(_rootDir, Year, Day, "input.example.txt");

    public Runner(string year, string day, string rootDir)
    {
        Year = year;
        Day = day;
        _rootDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(rootDir));
    }

    public async Task Run(bool exampleMode = false)
    {
        var solver = GetSolver();
        var filePath = exampleMode ? ExampleInputFilePath : InputFilePath;
        var input = await GetNormalizedInput(filePath);

        var stopwatch = Stopwatch.StartNew();
        var partOneAnswer = solver.PartOne(input);
        stopwatch.Stop();
        var elapsedPartOne = stopwatch.Elapsed;
        Console.WriteLine();
        Console.WriteLine($"{Year}/{Day} Part one {(exampleMode ? "(example) " : "")}answer (runtime of {elapsedPartOne.TotalMilliseconds}ms):");
        Console.WriteLine(partOneAnswer);

        stopwatch.Restart();
        var partTwoAnswer = solver.PartTwo(input);
        stopwatch.Stop();
        var elapsedPartTwo = stopwatch.Elapsed;
        Console.WriteLine($"{Year}/{Day} Part one {(exampleMode ? "(example) " : "")}answer (runtime of {elapsedPartTwo.TotalMilliseconds}ms):");
        Console.WriteLine(partTwoAnswer);

    }

    private ISolver GetSolver()
    {
        var solverPath = Path.Combine(_rootDir, Year, Day, "Solver.cs");
        if (!File.Exists(solverPath))
        {
            throw new Exception($"No solver at path {solverPath}");
        }

        var solverType = Assembly
            .GetEntryAssembly()!
            .GetTypes()
            .Where(t => t.GetTypeInfo().IsClass && typeof(ISolver).IsAssignableFrom(t))
            .First(t =>
            {
                var year = t.FullName!.Split('.').ToArray()[^3].Remove(0, 1);
                var day = t.FullName!.Split('.').ToArray()[^2].Remove(0, 1);
                return year == Year && day == Day;
            });

        return (Activator.CreateInstance(solverType) as ISolver)!;
    }

    private static async Task<string> GetNormalizedInput(string file)
    {
        var input = await File.ReadAllTextAsync(file);

        if (input.EndsWith("\n"))
        {
            return input[..^1];
        }
        return input;
    }
}
