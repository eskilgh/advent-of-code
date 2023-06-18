using System.Text.Json.Nodes;

namespace AdventOfCode.Y2022.D13;

internal class Solver : ISolver
{
  public string PartOne(string input)
  {
    var parsed = ParseInput(input);

    return parsed
      .Chunk(2)
      .Select((pair, i) => ComparePacketPair(pair[0], pair[1]) <= 0 ? i + 1 : 0)
      .Sum()
      .ToString();
  }

  private int ComparePacketPair(JsonNode left, JsonNode right)
  {
    if (left is JsonValue && right is JsonValue)
    {
      return (int)left - (int)right;
    }

    var l = left as JsonArray ?? new JsonArray((int)left);
    var r = right as JsonArray ?? new JsonArray((int)right);

    return Enumerable.Zip(l, r)
      .Select(pair => ComparePacketPair(pair.First, pair.Second))
      .FirstOrDefault(result => result != 0, l.Count - r.Count);
  }

  public string PartTwo(string input)
  {
    var parsed = ParseInput(input);
    var dividerA = JsonNode.Parse("[[2]]");
    var dividerB = JsonNode.Parse("[[6]]");
    parsed.Add(dividerA);
    parsed.Add(dividerB);

    parsed.Sort((l, r) => ComparePacketPair(l, r));

    return parsed.Select((line, i) => line == dividerA || line == dividerB ? i + 1 : 1)
      .Aggregate((a, b) => a * b)
      .ToString();
  }



  public static List<JsonNode> ParseInput(string input)
  {
    return input
      .Split("\n")
      .Where(line => !string.IsNullOrEmpty(line))
      .Select(line => JsonNode.Parse(line))
      .ToList();
  }
}
