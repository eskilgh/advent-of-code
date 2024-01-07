using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Y2023.D24;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Y2020.D04;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var passports = input
            .Split("\n\n")
            .Select(data =>
            {
                return data.Split(new char[] { ' ', '\n' })
                    .Select(pair => pair.Split(':'))
                    .ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            })
            .ToArray();

        string[] requiredFields = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];

        return passports.Count(passport => requiredFields.All(passport.Keys.Contains));
    }

    public object PartTwo(string input)
    {
        var passports = input
            .Split("\n\n")
            .Select(data =>
            {
                return data.Split(new char[] { ' ', '\n' })
                    .Select(pair => pair.Split(':'))
                    .ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            })
            .ToArray();

        string[] requiredFields = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];

        return passports.Where(passport => requiredFields.All(passport.Keys.Contains)).Count(Validate);
    }

    static bool Validate(Dictionary<string, string> passport)
    {
        string GenerateYearPattern(int minYear, int maxYear) =>
            $@"\b(?:{minYear}|{string.Join("|", Enumerable.Range(minYear + 1, maxYear - minYear - 1))}|{maxYear})\b";
        bool ValidateYear(string value, int minYear, int maxYear) =>
            Regex.IsMatch(value, GenerateYearPattern(minYear, maxYear));

        Func<string, bool> ValidateField(string field) =>
            field switch
            {
                "byr" => value => ValidateYear(value, 1920, 2002),
                "iyr" => value => ValidateYear(value, 2010, 2020),
                "eyr" => value => ValidateYear(value, 2020, 2030),
                "hgt" => value => Regex.IsMatch(value, @"\b((?:1[5-9]\d|19[0-3])cm|(?:5[9]|[6-7]\d)in)\b"),
                "hcl" => value => Regex.IsMatch(value, @"^#[0-9a-fA-F]{6}$"),
                "ecl" => value => Regex.IsMatch(value, @"^(amb|blu|brn|gry|grn|hzl|oth)$"),
                "pid" => value => Regex.IsMatch(value, @"^\d{9}$"),
                "cid" => value => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        return passport.All(kvp => ValidateField(kvp.Key)(kvp.Value));
    }
}
