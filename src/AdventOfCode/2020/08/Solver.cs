namespace AdventOfCode.Y2020.D08;

record Instruction(string Operation, int Argument);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var instructions = ParseInstructions(input);
        var (_, accumulator) = RunInstructionSet(instructions);

        return accumulator;
    }

    private static (bool RanToCompletion, int Accumulator) RunInstructionSet(Instruction[] instructions)
    {
        var accumulator = 0;
        var seen = new HashSet<int>();
        var i = 0;
        while (0 <= i && i < instructions.Length && seen.Add(i))
        {
            var instruction = instructions[i];
            Action action = instruction.Operation switch
            {
                "acc"
                    => () =>
                    {
                        accumulator += instruction.Argument;
                        i++;
                    },
                "jmp"
                    => () =>
                    {
                        i += instruction.Argument;
                    },
                "nop"
                    => () =>
                    {
                        i++;
                    },
                _ => throw new ArgumentOutOfRangeException()
            };
            action();
        }
        return (i == instructions.Length, accumulator);
    }

    private static Instruction[] ParseInstructions(string input)
    {
        return input
            .Split('\n')
            .Select(line =>
            {
                var operation = line.Split(' ')[0];
                var argument = int.Parse(line.Split(' ')[1]);
                return new Instruction(operation, argument);
            })
            .ToArray();
    }

    public object PartTwo(string input)
    {
        var instructions = ParseInstructions(input);
        var candidateIndices = instructions
            .Select((instruction, i) => (instruction, i))
            .Where(tpl => tpl.instruction.Operation is not "acc");

        foreach (var (instruction, i) in candidateIndices)
        {
            var swapped = instruction with { Operation = instruction.Operation is "nop" ? "jmp" : "nop" };
            instructions[i] = swapped;
            var (ranToCompletion, accumulator) = RunInstructionSet(instructions);
            if (ranToCompletion)
                return accumulator;
            instructions[i] = instruction;
        }

        throw new ArgumentOutOfRangeException(nameof(input), "Found no valid swap");
    }
}
