namespace AdventOfCode.Y2023.D25;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var rawEdges = input
            .Split('\n')
            .Select(line =>
            {
                var parts = line.Split(": ");
                var src = parts[0];
                var dests = parts[1].Split(' ').ToList();
                return (src, dests);
            })
            .ToList();

        var graph = new Dictionary<string, List<string>>();
        foreach (var (src, dests) in rawEdges)
        {
            if (graph.ContainsKey(src))
                graph[src].AddRange(dests);
            else
                graph[src] = dests;
            foreach (var dest in dests)
            {
                if (graph.ContainsKey(dest))
                    graph[dest].Add(src);
                else
                    graph[dest] = [src];
            }
        }

        var (minCut, s1Size, s2Size) = MinCut(graph);
        while (minCut != 3)
            (minCut, s1Size, s2Size) = MinCut(graph);

        return s1Size * s2Size;
    }

    (int size, int s1, int s2) MinCut(Dictionary<string, List<string>> graph)
    {
        var g = graph.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(v => v).ToList());
        var r = new Random();
        var sizes = g.Keys.ToDictionary(k => k, _ => 1);

        while (g.Count > 2)
        {
            var u = g.Keys.ElementAt(r.Next(g.Count));
            var v = g[u][r.Next(g[u].Count)];

            var mergedKey = $"{u}-{v}";
            g[mergedKey] = [.. g[u].Where(e => e != v), .. g[v].Where(e => e != u)];

            foreach (var key in g.Keys)
            {
                g[key] = g[key].Select(e => e == u || e == v ? mergedKey : e).ToList();
            }

            sizes[mergedKey] = sizes[u] + sizes[v];
            g.Remove(u);
            g.Remove(v);
        }

        return (g.Values.First().Count, sizes[g.Keys.First()], sizes[g.Keys.Last()]);
    }

    public object PartTwo(string input)
    {
        return "Not available";
    }
}
