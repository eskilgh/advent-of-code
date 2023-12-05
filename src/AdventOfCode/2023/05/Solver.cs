namespace AdventOfCode.Y2023.D05;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var inputSections = input.Split("\n\n");

        var seeds = inputSections[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1..].Select(long.Parse).ToArray();
        var maps = inputSections[1..].Select(ParseMap);

        var locationNumbers = new long[seeds.Length];
        seeds.CopyTo(locationNumbers, 0);

        for (var i = 0; i < seeds.Length; i++)
        {
            var currentLocation = locationNumbers[i];
            foreach (var map in maps)
            {
                var matchingMapping = map.FirstOrDefault(mapping =>
                {
                    var destination = mapping[0];
                    var source = mapping[1];
                    var range = mapping[2];
                    return source <= currentLocation && currentLocation < source + range;
                }, new long[] { 0, 0, 0 });

                var destination = matchingMapping[0];
                var source = matchingMapping[1];
                currentLocation = destination + currentLocation - source;   
            }
            locationNumbers[i] = currentLocation;
        }

        return locationNumbers.Min().ToString();
    }

    public static long[][] ParseMap(string input)
    {
        return input
            .Split('\n')[1..]
            .Select(line => line.Split(' ').Select(long.Parse).ToArray())
            .ToArray();
    }

    public string PartTwo(string input)
    {

        var inputSections = input.Split("\n\n");

        var seeds = inputSections[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1..].Select(long.Parse).ToArray();
        var maps = inputSections[1..].Select(ParseMap);

        var locationNumbers = new long[seeds.Length];
        seeds.CopyTo(locationNumbers, 0);

        for (var i = 0; i < seeds.Length; i++)
        {
            var currentLocation = locationNumbers[i];
            foreach (var map in maps)
            {
                var matchingMapping = map.FirstOrDefault(mapping =>
                {
                    var destination = mapping[0];
                    var source = mapping[1];
                    var range = mapping[2];
                    return source <= currentLocation && currentLocation < source + range;
                }, new long[] { 0, 0, 0 });

                var destination = matchingMapping[0];
                var source = matchingMapping[1];
                currentLocation = destination + currentLocation - source;
            }
            locationNumbers[i] = currentLocation;
        }

        return locationNumbers.Min().ToString();
    }
}
