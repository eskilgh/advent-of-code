using AngleSharp.Common;
using System.Text;

namespace AdventOfCode.Y2023.D20;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var modules = input
            .Split('\n')
            .Select(line =>
            {
                var parts = line.Split(" -> ");
                var moduleType = parts[0];
                var destinations = parts[1].Split(',', StringSplitOptions.TrimEntries).ToList();
                IModule module = moduleType switch
                {
                    "broadcaster" => new BroadCaster(destinations),
                    ['&', .. var name] => new Conjunction(name, destinations),
                    ['%', .. var name] => new FlipFlop(name, destinations),
                    _ => throw new ArgumentOutOfRangeException()
                };
                return module;
            })
            .ToDictionary(m => m.Name, m => m);
        foreach (var conjunction in modules.Values.Where(m  => m is Conjunction).Cast<Conjunction>())
        {
            conjunction.LastReceived = modules
                .Values
                .Where(m => m.Destinations.Contains(conjunction.Name))
                .ToDictionary(m => m.Name, _ => false);
        }
        var lowPulsesSent = 0;
        var highPulsesSent = 0;
        for (var i = 0; i < 1000; i++)
        {
            var queue = new Queue<(bool pulse, string destination, string source)>();
            queue.Enqueue((false, "broadcaster", "button"));
            while (queue.Count > 0)
            {
                var (pulse, destination, source) = queue.Dequeue();
                if (pulse)
                    highPulsesSent++;
                else
                    lowPulsesSent++;
                var signal = pulse ? "high" : "low";
                if (!modules.ContainsKey(destination))
                    continue;

                var (output, newDestinations) = modules[destination].Process(pulse, source);
                foreach (var newDest in newDestinations)
                {
                    queue.Enqueue((output, newDest, destination));
                }
            }
        }
        return (highPulsesSent * lowPulsesSent).ToString();
    }

    public string PartTwo(string input)
    {
        return "Not available";
    }
}

public class BroadCaster(List<string> destinations) : IModule
{
    public string Name => "broadcaster";
    public List<string> Destinations { get; } = destinations;

    public (bool Output, List<string> Destinations) Process(bool input, string source)
    {
        return (input, Destinations.ToList());
    }
}

public class Conjunction(string name, List<string> destinations) : IModule
{
    public string Name { get; } = name;
    public Dictionary<string, bool> LastReceived { get; set; } = null!;
    public List<string> Destinations { get; set; } = destinations;

    public (bool Output, List<string> Destinations) Process(bool input, string source)
    {
        LastReceived[source] = input;
        if (LastReceived.Values.All(highPulse => highPulse))
        {
            return (false, Destinations);
        }
        return (true, Destinations);
    }
}

public class FlipFlop(string name, List<string> destinations) : IModule
{
    public string Name { get; } = name;
    public List<string> Destinations { get; } = destinations;
    private bool isOn = false;

    public (bool Output, List<string> Destinations) Process(bool input, string source)
    {
        if (input)
            return (false, new List<string>());
        isOn = !isOn;
        return (isOn, Destinations);
    }
}

interface IModule
{
    public string Name { get; }
    public List<string> Destinations { get; }
    public (bool Output, List<string> Destinations) Process(bool input, string source);
}
