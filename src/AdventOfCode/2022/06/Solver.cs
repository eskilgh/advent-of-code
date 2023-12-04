namespace AdventOfCode.Y2022.D06;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return FirstUniqueSequence(input, 4).ToString();
    }

    public string PartTwo(string input)
    {
        return FirstUniqueSequence(input, 14).ToString();
    }

    private static int FirstUniqueSequence(string text, int sequenceLength)
    {
        var i = 0;
        while (i < text.Length - sequenceLength + 1)
        {
            var lastCollisionIdx = IndexOfLastCollision(text, i, i + sequenceLength - 1);
            if (lastCollisionIdx == -1)
                return i + sequenceLength;
            i = lastCollisionIdx + 1;
        }
        return -1;
    }

    /// <summary>
    /// Searches the text for character collisions from right to left, in the inclusive range [left, right].
    /// </summary>
    /// <returns>
    /// The index of the <c>leftmost</c> element in the first collision found, or -1 if no such element.
    /// </returns>
    private static int IndexOfLastCollision(string text, int left, int right)
    {
        if (left == right)
        {
            return -1;
        }
        var relativeIdx = text[left..right].LastIndexOf(text[right]);
        return relativeIdx == -1 ? IndexOfLastCollision(text, left, right - 1) : relativeIdx + left;
    }
}
