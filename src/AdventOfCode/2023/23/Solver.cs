using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D23;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var startCol = Array.IndexOf(m[0], '.');
        var endCol = Array.IndexOf(m[^1], '.');
        var start = new Vector2d(x: startCol, y: 0);
        var target = new Vector2d(x: endCol, y: m.Length - 1);

        var path = FindLongestPathNoLoops(m, start, target);
        return path.Count - 1;
    }

    List<Vector2d> FindLongestPathNoLoops(char[][] m, Vector2d start, Vector2d target)
    {
        var pq = new PriorityQueue<(Vector2d pos, Vector2d dir), int>();
        var height = m.Length;
        var width = m[0].Length;
        var paths = new Dictionary<Vector2d, List<Vector2d>> { [start] = new List<Vector2d> { start } };

        pq.Enqueue((start, Vector2d.Down), 0);
        while (pq.TryDequeue(out var posAndDir, out var length))
        {
            var (currentPos, currentDir) = posAndDir;
            var curr = m[currentPos.Y][currentPos.X];
            List<Vector2d> directions = curr switch
            {
                '.' => [Vector2d.Up, Vector2d.Right, Vector2d.Left, Vector2d.Down],
                '^' or 'v' or '>' or '<' => [ToDirection(curr)],
                _ => throw new ArgumentOutOfRangeException()
            };
            foreach (var nextDir in directions.Where(d => d != currentDir.Rotate180()))
            {
                var nextPos = currentPos + nextDir;
                var nextVal = nextPos.IsOutOfBounds(0, width - 1, 0, height - 1) ? '#' : m[nextPos.Y][nextPos.X];
                if (nextVal != '#')
                {
                    var existingPathAtNextPosIsGteNewPath =
                        paths.TryGetValue(nextPos, out var existingPathAtNextPos)
                        && existingPathAtNextPos.Count >= paths[currentPos].Count + 1;
                    if (!existingPathAtNextPosIsGteNewPath)
                    {
                        pq.Enqueue((nextPos, nextDir), length + 1);
                        paths[nextPos] = new List<Vector2d>(paths[currentPos]) { nextPos };
                    }
                }
            }
        }
        return paths[target];
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var startCol = Array.IndexOf(m[0], '.');
        var endCol = Array.IndexOf(m[^1], '.');
        var start = new Vector2d(x: startCol, y: 0);
        var target = new Vector2d(x: endCol, y: m.Length - 1);

        return FindLongestPath(m, start, target);
    }

    int FindLongestPath(char[][] m, Vector2d start, Vector2d target)
    {
        var nodes = MapSegments(m, start, target);
        return FindLongestPathRec(nodes);
    }

    /// <summary>
    /// Finds longest path between first and last element of nodes.
    /// </summary>
    int FindLongestPathRec((Vector2d node, (int connectedIndex, int length)[] connectedNodes)[] nodes)
    {
        var seen = nodes.Select(_ => false).ToArray();
        var lengths = nodes.Select(_ => 0).ToArray();

        void FindLongestPathInner(int nodeIndex, int currentLength)
        {
            var current = nodes[nodeIndex].node;
            if (seen[nodeIndex])
                return;

            seen[nodeIndex] = true;

            if (lengths![nodeIndex] < currentLength)
                lengths[nodeIndex] = currentLength;

            foreach (var connectedNode in nodes[nodeIndex].connectedNodes)
                FindLongestPathInner(connectedNode.connectedIndex, currentLength + connectedNode.length);

            seen[nodeIndex] = false;
        }

        FindLongestPathInner(0, 0);
        return lengths[^1];
    }

    /// <summary>
    /// Find all the nodes that are intersections, i.e. you can move more than 2 directions.
    /// The start and target node are also included.
    /// </summary>
    (Vector2d node, (int connectedIndex, int length)[] connectedNodes)[] MapSegments(
        char[][] m,
        Vector2d start,
        Vector2d target
    )
    {
        List<Vector2d> allDirections = [Vector2d.Up, Vector2d.Right, Vector2d.Left, Vector2d.Down];

        var nodes = (
            from row in m.Select((_, row) => row)
            from col in m[0].Select((_, col) => col)
            where
                m[row][col] != '#' && ValidDirections(m, new Vector2d(col, row)).Count() > 2
                || (col == start.X && row == start.Y)
                || (col == target.X && row == target.Y)
            select new Vector2d(col, row)
        ).ToArray();

        Array.Sort(
            nodes,
            (a, b) =>
            {
                var yComp = a.Y.CompareTo(b.Y);
                return yComp != 0 ? yComp : b.X.CompareTo(a.X);
            }
        );

        var seen = new Dictionary<(Vector2d pos, Vector2d dir), (Vector2d end, int length)>();
        return nodes
            .Select(node =>
            {
                var connectedEdges = ValidDirections(m, node)
                    .Select(dir =>
                    {
                        var connectedNode = FindNextNode(m, node, dir);
                        return (connectedIndex: Array.IndexOf(nodes, connectedNode.node), length: connectedNode.length);
                    })
                    .ToArray();
                return (node, connectedEdges);
            })
            .ToArray();
    }

    IEnumerable<Vector2d> ValidDirections(char[][] m, Vector2d pos, Vector2d currentDir = default)
    {
        List<Vector2d> allDirections = [Vector2d.Up, Vector2d.Right, Vector2d.Left, Vector2d.Down];

        return allDirections
            .Where(dir =>
            {
                var newPos = dir + pos;
                return !newPos.IsOutOfBounds(m) && m[newPos.Y][newPos.X] != '#';
            })
            .Where(dir => currentDir == default || dir != currentDir.Rotate180());
    }

    (Vector2d node, Vector2d dir, int length) FindNextNode(char[][] m, Vector2d pos, Vector2d dir)
    {
        var currentPos = pos + dir;
        var currentDir = dir;
        var length = 1;
        var validDirections = ValidDirections(m, currentPos, currentDir);
        while (validDirections.Count() == 1)
        {
            currentDir = validDirections.Single();
            currentPos += currentDir;
            length++;
            validDirections = ValidDirections(m, currentPos, currentDir);
        }

        return (currentPos, currentDir.Rotate180(), length);
    }

    Vector2d ToDirection(char c) =>
        c switch
        {
            '^' => Vector2d.Up,
            '>' => Vector2d.Right,
            '<' => Vector2d.Left,
            'v' => Vector2d.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(c))
        };
}
