namespace AdventOfCode.Y2023.D05;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var inputSections = input.Split("\n\n");

        var seeds = inputSections[0]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1..]
            .Select(long.Parse)
            .ToArray();
        var maps = inputSections[1..].Select(ParseMap);

        var locationNumbers = new long[seeds.Length];
        seeds.CopyTo(locationNumbers, 0);

        for (var i = 0; i < seeds.Length; i++)
        {
            var currentLocation = locationNumbers[i];
            foreach (var map in maps)
            {
                var matchingMapping = map.FirstOrDefault(
                    mapping =>
                    {
                        var destination = mapping[0];
                        var source = mapping[1];
                        var range = mapping[2];
                        return source <= currentLocation && currentLocation < source + range;
                    },
                    new long[] { 0, 0, 0 }
                );

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
        return input.Split('\n')[1..].Select(line => line.Split(' ').Select(long.Parse).ToArray()).ToArray();
    }

    public string PartTwo(string input)
    {
        var inputSections = input.Split("\n\n");
        var seedRanges = inputSections[0]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1..]
            .Select(long.Parse)
            .Chunk(2)
            .ToList();

        var currentLocationRanges = new List<long[]>(seedRanges);
        var maps = inputSections[1..].Select(ParseMap);

        foreach (var map in maps)
        {
            var newLocationRanges = new List<long[]>();
            foreach (var locationRange in currentLocationRanges)
            {
                newLocationRanges.AddRange(MapToNewLocations(locationRange[0], locationRange[1], map));
            }
            currentLocationRanges = newLocationRanges;
        }
        return currentLocationRanges.Min(x => x[0]).ToString();
    }

    public static List<long[]> MapToNewLocations(long originalLocation, long originalLength, long[][] map)
    {
        var newLocations = new List<long[]>();
        var matchingMapping = map.FirstOrDefault(mapping =>
        {
            var destination = mapping[0];
            var source = mapping[1];
            var range = mapping[2];

            if (originalLocation <= source && source < (originalLocation + originalLength))
                return true;
            if (source <= originalLocation && originalLocation < (source + range))
                return true;

            return false;
        });

        if (matchingMapping is null)
        {
            newLocations.Add(new long[] { originalLocation, originalLength });
            return newLocations;
        }

        var destination = matchingMapping[0];
        var source = matchingMapping[1];
        var range = matchingMapping[2];

        var leftLength = Math.Max(source - originalLocation, 0);
        if (leftLength > 0)
        {
            newLocations.AddRange(MapToNewLocations(originalLocation, leftLength, map));
        }

        var rightLength = Math.Max(originalLocation + originalLength - (source + range), 0);
        if (rightLength > 0)
        {
            var rightStart = originalLocation + originalLength - rightLength;
            newLocations.AddRange(MapToNewLocations(rightStart, rightLength, map));
        }
        var midLength = originalLength - leftLength - rightLength;
        var midStart = originalLocation <= source ? destination : destination + originalLocation - source;
        newLocations.Add(new long[] { midStart, midLength });
        return newLocations;
    }
}
