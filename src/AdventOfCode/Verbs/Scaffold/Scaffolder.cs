using System.Globalization;
using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;

namespace AdventOfCode.Verbs.Scaffold;

public class Scaffolder
{
  public readonly string Year;
  public readonly string Day;
  private readonly string _rootDir;
  private readonly Url baseUrl = new(new Uri("http://adventofcode.com").ToString());
  private string TargetDir => Path.Combine(_rootDir, Year, Day);

  public Scaffolder(string year, string day, string rootDir)
  {
    Year = year;
    Day = day;
    _rootDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(rootDir));
  }

  public async Task Run()
  {
    await ScaffoldSolver();
    await FetchAndWriteInput();
  }

  public async Task FetchAndWriteInput()
  {
    var context = GetContext();
    var input = await GetInput(context);
    var inputPath = Path.Combine(TargetDir, "input.txt");
    await File.WriteAllTextAsync(inputPath, input);

    var exampleInput = await GetExampleInput(context);
    var exampleInputPath = Path.Combine(TargetDir, "input.example.txt");
    await File.WriteAllTextAsync(exampleInputPath, exampleInput);
  }

  private async Task<string> GetExampleInput(IBrowsingContext context)
  {
    var dayWithoutLeadingZeroes = int.Parse(Day, NumberStyles.Any);

    var problemStatement = await context.OpenAsync(new Url(baseUrl, $"{Year}/day/{dayWithoutLeadingZeroes}"));
    return problemStatement.QuerySelector("code")!.InnerHtml;
  }

  private async Task<string> GetInput(IBrowsingContext context)
  {
    var dayWithoutLeadingZeroes = int.Parse(Day, NumberStyles.Any);
    var inputResponse = await context.GetService<IDocumentLoader>()!.FetchAsync(
            new DocumentRequest(new Url(baseUrl, $"{Year}/day/{dayWithoutLeadingZeroes}/input"))).Task;

    if (inputResponse.StatusCode != HttpStatusCode.OK)
    {
      throw new Exception($"Could not fetch input, got response status code: {inputResponse.StatusCode}");
    }
    return new StreamReader(inputResponse.Content).ReadToEnd();
  }

  private IBrowsingContext GetContext()
  {
    var session = GetSession();
    var requester = new DefaultHttpRequester("github.com/eskilgh/aoc");
    var config = Configuration.Default
      .With(requester)
      .WithDefaultLoader()
      .WithDefaultCookies();

    var context = BrowsingContext.New(config);
    context.SetCookie(baseUrl, $"session={session}");
    return context;
  }

  public async Task ScaffoldSolver()
  {
    if (!Directory.Exists(TargetDir))
    {
      Directory.CreateDirectory(TargetDir);
    }
    var solverTemplate = SolverTemplateGenerator.Generate(year: Year, day: Day);
    var solverPath = Path.Combine(TargetDir, "Solver.cs");
    await File.WriteAllTextAsync(solverPath, solverTemplate);
  }

  private static string GetSession()
  {
    var session = Environment.GetEnvironmentVariable("AOC_SESSION");
    if (string.IsNullOrEmpty(session))
    {
      throw new Exception("AOC_SESSION environment variable not specified");
    }
    return session;
  }
}
