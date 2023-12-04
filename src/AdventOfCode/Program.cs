using System.Linq;
using System.Text;
using AdventOfCode;
using AdventOfCode.Verbs.Run;
using AdventOfCode.Verbs.Scaffold;
using CommandLine;

// See https://aka.ms/new-console-template for more information

await Parser
    .Default
    .ParseArguments<ScaffoldOptions, RunOptions>(args)
    .MapResult(
        (ScaffoldOptions opts) => HandleScaffold(opts),
        (RunOptions opts) => HandleRun(opts),
        errs => HandleParseError(errs)
    );

static async Task<int> HandleScaffold(ScaffoldOptions opts)
{
    var year = opts.Date.Split('/')[0];
    var day = opts.Date.Split('/')[1];
    var scaffolder = new Scaffolder(year: year, day: day, rootDir: Environment.CurrentDirectory);
    try
    {
        await scaffolder.Run();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        Console.Error.WriteLine();
        Console.Error.WriteLine(ex.StackTrace);
        return 1;
    }
    return 0;
}

static async Task<int> HandleRun(RunOptions opts)
{
    var year = opts.Date.Split('/')[0];
    var day = opts.Date.Split('/')[1];
    var runner = new Runner(year: year, day: day, rootDir: Environment.CurrentDirectory);
    try
    {
        await runner.Run(opts.Example);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine();
        Console.Error.WriteLine(ex.StackTrace);
        return 1;
    }
    return 0;
}

static Task<int> HandleParseError(IEnumerable<Error> errs)
{
    foreach (var error in errs)
    {
        Console.Error.WriteLine(error);
    }
    return Task.FromResult(1);
}
