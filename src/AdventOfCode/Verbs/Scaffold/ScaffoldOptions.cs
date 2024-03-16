using System;
using System.ComponentModel;
using CommandLine;

namespace AdventOfCode.Verbs.Scaffold;

[Verb("scaffold", HelpText = "Scaffolds a solution directory for the provided date.")]
public class ScaffoldOptions
{
    [Value(0, MetaName = "Date", HelpText = "Date to scaffold, format: YYYY/DD.")]
    public required string Date { get; set; }

    [Option(
        'o',
        "overwrite",
        Required = false,
        HelpText = "Will overwrite in case of file collisions"
    )]
    public bool Overwrite { get; set; }

    [Option(
        'l',
        "language",
        Required = false,
        Default = Language.CSharp,
        HelpText = "The language of the solver template to create."
    )]
    public Language Language { get; set; }
}