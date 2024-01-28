using OneOf;

namespace AdventOfCode.Y2020.D18;

record Node(Node? Left, Node? Right, OneOf<char, long> Value);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return Parse(input, withOperatorPrecedence: false).Select(Evaluate).Sum();
    }

    public object PartTwo(string input)
    {
        return Parse(input, withOperatorPrecedence: true).Select(Evaluate).Sum();
    }

    static IEnumerable<Node> Parse(string input, bool withOperatorPrecedence)
    {
        return input.Split('\n').Select(line => ToExpressionTree(line, withOperatorPrecedence)).ToList();
    }

    static long Evaluate(Node node)
    {
        long Add(Node left, Node right) => Evaluate(left) + Evaluate(right);
        long Multiply(Node left, Node right) => Evaluate(left) * Evaluate(right);
        return node switch
        {
            { Left: null, Right: null, Value: var number } when number.IsT1 => number.AsT1,
            { Left: Node left, Right: Node right, Value: var @operator } when @operator.IsT0
                => @operator.AsT0 switch
                {
                    '+' => Add(left, right),
                    '*' => Multiply(left, right),
                    _ => throw new ArgumentException("Invalid operator")
                },
            _ => throw new ArgumentException("Invalid tree", nameof(node))
        };
    }

    // Parses a single line expression into a tree of nodes.
    // If parsing with precedence, will produce a tree where addition has precedence over multiplication.
    // Otherwise, a operators will have the same precedence. 
    //          "8 * 3 + 9 * 4" becomes:
    //   with precedence |     without precedence
    //       4           |          *             
    //      /            |         / \            
    //     *             |        +   4           
    //    / \            |       / \              
    //   8   +           |      *   9             
    //      / \          |     / \                
    //     3   9         |    8   3               
    static Node ToExpressionTree(string expression, bool withOperatorPrecedence = true)
    {
        var parts = Tokenize(expression);
        var enumerator = parts.GetEnumerator();
        var (left, @operator) = ParseSubExpression(enumerator, withOperatorPrecedence);
        while (@operator.HasValue)
        {
            var (right, nextOperator) = ParseSubExpression(enumerator, withOperatorPrecedence);
            left = new Node(left, right, @operator.Value);
            @operator = nextOperator;
        }
        return left;
    }

    static (Node, char? nextOperator) ParseSubExpression(
        IEnumerator<OneOf<string, char, long>> enumerator,
        bool withOperatorPrecedence
    )
    {
        if (!enumerator.MoveNext())
            throw new ArgumentException("invalid expression");

        var token = enumerator.Current;
        var left = token.Match(
            subExpression => ToExpressionTree(subExpression, withOperatorPrecedence),
            _ => throw new ArgumentException(),
            number => new Node(Left: null, Right: null, Value: number)
        );

        if (!enumerator.MoveNext())
            return (left, null);

        var @operator = enumerator.Current.AsT1;
        if (@operator is '*' || !withOperatorPrecedence)
            return (left, @operator);

        if (@operator is '+')
        {
            var (right, nextOperator) = ParseSubExpression(enumerator, withOperatorPrecedence);
            return (new Node(left, right, @operator), nextOperator);
        }
        throw new ArgumentOutOfRangeException();
    }

    static IEnumerable<OneOf<string, char, long>> Tokenize(string s)
    {
        var stripped = s.Replace(" ", string.Empty);
        var i = 0;
        while (i < stripped.Length)
        {
            var c = stripped[i];

            switch (c)
            {
                case var _ when char.IsDigit(c):
                    var num = string.Join(string.Empty, stripped[i..].TakeWhile(char.IsDigit));
                    yield return (long.Parse(num));
                    i += num.Length;
                    break;

                case '*':
                case '+':
                    yield return c;
                    i++;
                    break;

                case '(':
                    var matchingIndex = FindMatchingParenthesis(stripped, i + 1);
                    var subExpression = stripped.Substring(i + 1, matchingIndex - i - 1);
                    yield return subExpression;
                    i = matchingIndex + 1;
                    break;

                default:
                    throw new ArgumentException("Invalid expression");
            }
        }
    }

    /// <summary>
    /// Finds the closing parenthesis that matches the left parenthesis at index - 1
    /// </summary>
    /// <param name="s">string to search</param>
    /// <param name="index">index to start searching from</param>
    static int FindMatchingParenthesis(string s, int index)
    {
        var parenthesisCount = 1;
        var i = index;
        while (i < s.Length)
        {
            if (s[i] is '(')
                parenthesisCount++;
            if (s[i] is ')' && --parenthesisCount == 0)
                return i;
            i++;
        }
        return -1;
    }
}
