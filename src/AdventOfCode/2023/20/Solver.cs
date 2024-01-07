using System.Text;
using AngleSharp.Common;

namespace AdventOfCode.Y2023.D20;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var modules = ParseModules(input);
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
                if (!modules.ContainsKey(destination))
                    continue;

                var (output, newDestinations) = modules[destination].Process(pulse, source);
                foreach (var newDest in newDestinations)
                {
                    queue.Enqueue((output, newDest, destination));
                }
            }
        }
        return highPulsesSent * lowPulsesSent;
    }

    public object PartTwo(string input)
    {
        var modules = ParseModules(input);
        
        // This is by no ways a general solution: After inspecting my input, I found that in order
        // for the rx to receive a low pulse, there were four conjunction modules that needed
        // to send a high pulse at the same time (see the cycles dict).
        // Further testing revealed that each of these would output a high signal at a fixed cycle.
        // So the solution is just to identify the cycle length of each of these, and then find when they
        // will sync up (using LCM).

        var cycles = new Dictionary<string, long>
        {
            { "lz", -1 },
            { "tg", -1 },
            { "hn", -1 },
            { "kh", -1 }
        };

        var i = 1;
        while (cycles.Values.Any(v => v == -1))
        {
            var queue = new Queue<(bool pulse, string destination, string source)>();
            queue.Enqueue((false, "broadcaster", "button"));
            while (queue.Count > 0)
            {
                var (pulse, destination, source) = queue.Dequeue();
                if (!modules.ContainsKey(destination))
                    continue;

                var (output, newDestinations) = modules[destination].Process(pulse, source);
                if (output && modules[destination] is Conjunction conjunction && cycles.ContainsKey(conjunction.Name))
                {
                    if (cycles[conjunction.Name] == -1)
                        cycles[conjunction.Name] = i;
                }
                foreach (var newDest in newDestinations)
                {
                    queue.Enqueue((output, newDest, destination));
                }
            }
            i++;
        }

        return cycles.Values.Aggregate(LCM);
    }

    static Dictionary<string, IModule> ParseModules(string input)
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

        foreach (var conjunction in modules.Values.Where(m => m is Conjunction).Cast<Conjunction>())
        {
            conjunction.LastReceived = modules
                .Values
                .Where(m => m.Destinations.Contains(conjunction.Name))
                .ToDictionary(m => m.Name, _ => false);
        }
        return modules;
    }

    static long LCM(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }

    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}

interface IModule
{
    public string Name { get; }
    public List<string> Destinations { get; }
    public (bool Output, List<string> Destinations) Process(bool input, string source);
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
