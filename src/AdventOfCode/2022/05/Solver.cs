namespace AdventOfCode.Y2022.D05;

internal class Solver : ISolver
{
  public string PartOne(string input)
  {
    var stacks = ParseStacks(input.Split("\n\n")[0]);
    foreach (var instruction in input.Split("\n\n")[1].Split("\n"))
    {
      DoInstruction9000(instruction, stacks);
    }
    return new string(stacks.Select(stack => stack.Pop()).ToArray());
  }

  public string PartTwo(string input)
  {
    var stacks = ParseStacks(input.Split("\n\n")[0]);
    foreach (var instruction in input.Split("\n\n")[1].Split("\n"))
    {
      DoInstruction9001(instruction, stacks);
    }
    return new string(stacks.Select(stack => stack.Pop()).ToArray());
  }

  private static Stack<char>[] ParseStacks(string input)
  {
    var lines = input.Split("\n");
    var numStacks = Convert.ToInt32(Math.Ceiling(lines[0].Length * 1.0 / 4));
    var stacks = (new Stack<char>[numStacks]).Select(_ => new Stack<char>()).ToArray();
    var lettersWithStackIndices = lines
      .Take(lines.Length - 1)
      .Select(line => line.Chunk(4))
      .Select((chunks) => chunks
        .Select((cs, i) => (Chars: cs, StackIdx: i))
        .Where(e => !e.Chars.All((c) => c == ' '))
        .Select(e => (Letter: e.Chars[1], e.StackIdx)))
      .Reverse();

    foreach (var line in lettersWithStackIndices)
    {
      foreach (var (Letter, StackIdx) in line)
      {
        stacks[StackIdx].Push(Letter);
      }
    }
    return stacks;
  }

  private static void DoInstruction9000(string instruction, Stack<char>[] stacks)
  {
    var ss = instruction.Split(" ");
    var reps = int.Parse(ss[1]);
    var src = stacks[int.Parse(ss[3]) - 1];
    var dest = stacks[int.Parse(ss[5]) - 1];
    for (var i = 0; i < reps; i++)
    {
      dest.Push(src.Pop());
    }
  }

  private static void DoInstruction9001(string instruction, Stack<char>[] stacks)
  {
    var ss = instruction.Split(" ");
    var reps = int.Parse(ss[1]);
    var src = stacks[int.Parse(ss[3]) - 1];
    var dest = stacks[int.Parse(ss[5]) - 1];

    var tmp = new Stack<char>();
    for (var i = 0; i < reps; i++)
    {
      tmp.Push(src.Pop());
    }
    for (var i = 0; i < reps; i++)
    {
      dest.Push(tmp.Pop());
    }
  }


}
