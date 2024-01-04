namespace AdventOfCode.Y2022.D11;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var monkeys = input.Split("\n\n").Select(ParseMonkey).ToArray();
        foreach (var _ in Enumerable.Range(0, 20))
        {
            DoRound(monkeys, maxValue: int.MaxValue, shouldDivide: true);
        }
        var monkeyBusinessLevel = monkeys
            .Select(m => m.NumInspections)
            .OrderByDescending(numInspections => numInspections)
            .Take(2)
            .Aggregate(1, (a, b) => a * b);
        return monkeyBusinessLevel;
    }

    public object PartTwo(string input)
    {
        var maxValue = input
            .Split("\n")
            .Where(line => line.StartsWith("  Test:"))
            .Select(testLine => int.Parse(testLine.Split(" ").Last()))
            .Aggregate(1, (a, b) => a * b);
        var monkeys = input.Split("\n\n").Select(ParseMonkey).ToArray();
        foreach (var _ in Enumerable.Range(0, 10000))
        {
            DoRound(monkeys, maxValue: maxValue, shouldDivide: false);
        }
        var monkeyBusinessLevel = monkeys
            .Select(m => m.NumInspections)
            .OrderByDescending(numInspections => numInspections)
            .Take(2)
            .Aggregate(1, (a, b) => a * b);
        return monkeyBusinessLevel;
    }

    private static void DoRound(Monkey[] monkeys, int maxValue, bool shouldDivide)
    {
        for (var i = 0; i < monkeys.Length; i++)
        {
            var monkey = monkeys[i];
            while (monkey.TryThrow(monkeys, maxValue: maxValue, shouldDivide)) { }
        }
    }

    private static Monkey ParseMonkey(string monkeyInput)
    {
        var lines = monkeyInput.Split("\n");
        var startingItems = new Queue<int>(lines[1].Split(": ")[^1].Split(", ").Select(int.Parse));
        var operation = ParseOperation(lines[2]);
        bool test(int worryLevel) => (worryLevel % int.Parse(lines[3].Split(" ")[^1])) == 0;
        var trueDirection = int.Parse(lines[4].Split(" ")[^1]);
        var falseDirection = int.Parse(lines[5].Split(" ")[^1]);
        return new Monkey(
            Items: startingItems,
            Operation: operation,
            Test: test,
            TrueDirection: trueDirection,
            FalseDirection: falseDirection
        );
    }

    private static Func<int, int, int> ParseOperation(string operationInput)
    {
        var expression = operationInput.Split("= ")[^1].Split(' ');
        var ParseValue = (int old) => (string s) => s == "old" ? old : int.Parse(s);
        return (int old, int maxValue) =>
        {
            var a = ParseValue(old)(expression[0]);
            var b = ParseValue(old)(expression[^1]);
            return expression[1] switch
            {
                "+" => (a + b) % maxValue,
                "*" => a * b % maxValue,
                _ => throw new ArgumentOutOfRangeException(nameof(operationInput))
            };
        };
    }
}

public record Monkey(
    Queue<int> Items,
    Func<int, int, int> Operation,
    Predicate<int> Test,
    int TrueDirection,
    int FalseDirection
)
{
    public int NumInspections { get; private set; }

    public bool TryThrow(Monkey[] monkeys, int maxValue, bool shouldDivide)
    {
        if (!Items.TryDequeue(out var item))
            return false;

        var newItem = Operation(item, maxValue);
        if (shouldDivide)
            newItem /= 3;
        var receiverIndex = Test(newItem) ? TrueDirection : FalseDirection;
        monkeys[receiverIndex].Receive(newItem);
        NumInspections++;
        return true;
    }

    public void Receive(int item)
    {
        Items.Enqueue(item);
    }
}
