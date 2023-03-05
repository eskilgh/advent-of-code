using CommandLine;
using AdventOfCode;
using System.Text;
using System.Linq;
using AdventOfCode.Verbs.Scaffold;
using AdventOfCode.Verbs.Run;
// See https://aka.ms/new-console-template for more information

await Parser.Default.ParseArguments<ScaffoldOptions, RunOptions>(args)
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
    Console.Error.WriteLine(ex.StackTrace);
    Console.WriteLine(ex.Message);
    return 1;
  }
  return 0;
}

static async Task<int> HandleRun(RunOptions opts)
{
  var year = opts.Date.Split('/')[0];
  var day = opts.Date.Split('/')[1];
  var runner = new Runner(year: year, day: day, rootDir: Environment.CurrentDirectory);
  await runner.Run(opts.Example);
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