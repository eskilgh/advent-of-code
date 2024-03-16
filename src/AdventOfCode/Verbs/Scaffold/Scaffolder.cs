using System.Globalization;
using System.Net;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using static AdventOfCode.Common.Utils;

namespace AdventOfCode.Verbs.Scaffold;

public class Scaffolder
{
    public readonly string Year;
    public readonly string Day;
    private readonly bool _overwrite;
    private readonly Language _language;

    private readonly Url baseUrl = new(new Uri("http://adventofcode.com").ToString());
    private string CSharpTargetDir => Path.Combine(GetCurrentProjectRootPath(), Year, Day);

    private string FSharpProjectRoot =>
        Path.Combine(Directory.GetParent(GetCurrentProjectRootPath())!.FullName, "FSharpSolutions");

    public Scaffolder(string year, string day, bool overwrite, Language language)
    {
        Year = year;
        Day = day;
        _overwrite = overwrite;
        _language = language;
    }

    public async Task Run()
    {
        await FetchAndWriteInput();
        await (_language switch
        {
            Language.CSharp => ScaffoldCSharpSolver(),
            Language.FSharp => ScaffoldFSharpSolver()
        });
    }

    public async Task FetchAndWriteInput()
    {
        var context = GetContext();
        var input = await GetInput(context);
        var inputPath = Path.Combine(CSharpTargetDir, "input.txt");
        await WriteToFile(path: inputPath, content: input);

        var exampleInput = await GetExampleInput(context);
        if (exampleInput is null)
        {
            Console.WriteLine("Failed scraping example input");
            return;
        }
        var exampleInputPath = Path.Combine(CSharpTargetDir, "input.example.txt");
        await WriteToFile(path: exampleInputPath, content: exampleInput);
    }

    private async Task<string?> GetExampleInput(IBrowsingContext context)
    {
        var dayWithoutLeadingZeroes = int.Parse(Day, NumberStyles.Any);

        var problemStatement = await context.OpenAsync(new Url(baseUrl, $"{Year}/day/{dayWithoutLeadingZeroes}"));
        return problemStatement.QuerySelector("pre code")?.InnerHtml;
    }

    private async Task<string> GetInput(IBrowsingContext context)
    {
        var dayWithoutLeadingZeroes = int.Parse(Day, NumberStyles.Any);
        var inputResponse = await context
            .GetService<IDocumentLoader>()!
            .FetchAsync(new DocumentRequest(new Url(baseUrl, $"{Year}/day/{dayWithoutLeadingZeroes}/input")))
            .Task;

        if (inputResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Could not fetch input, got response status code: {inputResponse.StatusCode}");
        }
        return await new StreamReader(inputResponse.Content).ReadToEndAsync();
    }

    private IBrowsingContext GetContext()
    {
        var session = GetSession();
        var requester = new DefaultHttpRequester("github.com/eskilgh/aoc");
        var config = Configuration.Default.With(requester).WithDefaultLoader().WithDefaultCookies();

        var context = BrowsingContext.New(config);
        context.SetCookie(baseUrl, $"session={session}");
        return context;
    }

    public async Task ScaffoldCSharpSolver()
    {
        var solverTemplate = SolverTemplateGenerator.Generate(year: Year, day: Day);
        var solverPath = Path.Combine(CSharpTargetDir, "Solver.cs");
        var didWrite = await WriteToFile(content: solverTemplate, path: solverPath);
        if (!didWrite)
        {
            Console.WriteLine(
                $"Solver file already exists at {solverPath}. (Use --overwrite option to force overwrite"
            );
        }
    }

    public async Task ScaffoldFSharpSolver()
    {
        var solverTemplate = SolverTemplateGenerator.GenerateFSharp(year: Year, day: Day);
        var solverSubPath = Path.Combine(Year, $"Day{Day}.fs");
        var solverPath = Path.Combine(FSharpProjectRoot, solverSubPath);
        var didWrite = await WriteToFile(content: solverTemplate, path: solverPath);
        if (didWrite)
        {
            AddModuleToFSharpProject(
                Path.Combine(FSharpProjectRoot, "FSharpSolutions.fsproj"),
                solverSubPath
                );
        }
        else
        {
            Console.WriteLine(
                $"Solver file already exists at {solverPath}. (Use --overwrite option to force overwrite"
            );
        }
    }

    static void AddModuleToFSharpProject(string fsprojPath, string modulePath)
    {
        var projectXml = XDocument.Load(fsprojPath);

        var compileElement = new XElement("Compile");
        compileElement.SetAttributeValue("Include", modulePath);

        var itemGroup = projectXml.Root?.Elements("ItemGroup").Single(e => e.Attribute("Label")?.Value == "Modules" ) ?? throw new ArgumentException();

        var alreadyAdded = itemGroup.Elements("Compile").Any(e => e.Attribute("Include")?.Value == modulePath);
        if (alreadyAdded)
            return;

        itemGroup.Add(compileElement);

        projectXml.Save(fsprojPath);
    }

    private async Task<bool> WriteToFile(string content, string path)
    {
        if (File.Exists(path) && !_overwrite)
            return false;
        var dirName = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dirName) && !Directory.Exists(dirName))
            Directory.CreateDirectory(dirName);

        await File.WriteAllTextAsync(path, content);
        return true;
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
