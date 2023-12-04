using System;
using CommandLine;

namespace AdventOfCode.Verbs.Run;

[Verb("run", HelpText = "Runs the solution for the provided date.")]
class RunOptions
{
    [Value(0, MetaName = "Date", HelpText = "Date to run, format: YYYY/DD.")]
    public required string Date { get; set; }

    [Option(
        'e',
        "example",
        Required = false,
        HelpText = "Runs the solution with the example input from the problem prompt."
    )]
    public bool Example { get; set; }
}
