using System;
using System.Diagnostics;
using System.Reflection;
using AdventOfCode.Y2020.D01;
using Microsoft.FSharp.Reflection;
using static AdventOfCode.Common.Utils;

namespace AdventOfCode.Verbs.Run;

public class Runner
{
    public readonly string Year;
    public readonly string Day;
    private readonly string _rootDir;
    private readonly Language _language;

    private string InputFilePath => Path.Combine(_rootDir, Year, Day, "input.txt");
    private string ExampleInputFilePath => Path.Combine(_rootDir, Year, Day, "input.example.txt");

    public Runner(string year, string day, Language language)
    {
        Year = year;
        Day = day;
        _rootDir = GetCurrentProjectRootPath();
        _language = language;
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
        Console.WriteLine($"{Year}/{Day} Part two {(exampleMode ? "(example) " : "")}answer (runtime of {elapsedPartTwo.TotalMilliseconds}ms):");
        Console.WriteLine(partTwoAnswer);
    }

    private ISolver GetSolver()
    {
        return _language switch
        {
            Language.CSharp => GetCSharpSolver(),
            Language.FSharp => GetFSharpSolver(),
        };
    }

    private ISolver GetFSharpSolver()
    {
        var assembly = Assembly.Load("FSharpSolutions");
        var @namespace = $"FSharpSolutions.Y{Year}.D{Day}";
        var solverType = assembly.GetType($"{@namespace}.Solver")!;
        return (Activator.CreateInstance(solverType) as ISolver)!;
    }

    private ISolver GetCSharpSolver()
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
