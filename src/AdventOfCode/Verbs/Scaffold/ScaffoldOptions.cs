using System;
using CommandLine;

namespace AdventOfCode.Verbs.Scaffold;

[Verb("scaffold", HelpText = "Scaffolds a solution directory for the provided date.")]
public class ScaffoldOptions
{
  [Value(0, MetaName = "Date", HelpText = "Date to scaffold, format: YYYY/DD.")]
  public required string Date { get; set; }
}