using System;
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
    var partOneAnswer = solver.PartOne(input);
    var partTwoAnswer = solver.PartTwo(input);

    Console.WriteLine();
    Console.WriteLine($"{Year}/{Day} Answer part one:");
    Console.WriteLine(partOneAnswer);
    Console.WriteLine($"{Year}/{Day} Answer part two:");
    Console.WriteLine(partTwoAnswer);
  }

  private ISolver GetSolver()
  {
    var solverPath = Path.Combine(_rootDir, Year, Day, "Solver.cs");
    if (!File.Exists(solverPath))
    {
      throw new Exception($"No solver at path {solverPath}");
    }

    var solverType = Assembly.GetEntryAssembly()!.GetTypes()
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