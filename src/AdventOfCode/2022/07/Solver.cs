using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AdventOfCode.Y2022.D07;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var root = ParseFileSystem(input);
        return root.Where(entry => entry is Directory && entry.Size < 100000).Sum(dir => dir.Size);
    }

    public object PartTwo(string input)
    {
        var root = ParseFileSystem(input);
        var unusedSpace = 70000000 - root.Size;
        var minCandidateSize = 30000000 - unusedSpace;
        return root.Where(entry => entry is Directory)
            .Aggregate(
                int.MaxValue,
                (candidate, current) => current.Size < minCandidateSize ? candidate : Math.Min(current.Size, candidate)
            );
    }

    private static Directory ParseFileSystem(string input)
    {
        var lines = input.Split("\n");
        var root = new Directory() { Name = lines[0].Split(" ")[2], Parent = null };
        var cd = root;

        foreach (var line in input.Split("\n")[1..])
        {
            if (line.StartsWith("$ cd"))
            {
                var newDirName = line.Split(" ")[2];
                if (newDirName == "..")
                    cd = cd.Parent!;
                else if (cd.Children.FirstOrDefault(entry => entry.Name == newDirName) is Directory dir)
                    cd = dir;
                else
                {
                    var newDir = new Directory { Name = newDirName, Parent = cd };
                    cd.Children.Add(newDir);
                    cd = newDir;
                }
                continue;
            }
            if (line.StartsWith("$ ls"))
                continue;
            if (line.StartsWith("dir"))
                continue;
            var file = new File
            {
                Parent = cd,
                Name = line.Split(" ")[1],
                FileSize = int.Parse(line.Split(" ")[0])
            };
            cd.Children.Add(file);
        }
        return root;
    }
}

public abstract class FileSystemEntry
{
    [Required]
    public required string Name { get; init; }
    public Directory? Parent { get; set; }
    public abstract int Size { get; }

    public override string ToString() => $"{Name} - {Size}";
}

public class File : FileSystemEntry
{
    public override int Size
    {
        get => FileSize;
    }
    public int FileSize { get; set; }
}

public class Directory : FileSystemEntry, IEnumerable<FileSystemEntry>
{
    public IList<FileSystemEntry> Children { get; private set; } = new List<FileSystemEntry>();
    private int? _size;
    public override int Size
    {
        get
        {
            if (_size.HasValue)
                return _size.Value;
            _size = Children.Select(c => c.Size).Sum();
            return _size.Value;
        }
    }

    public IEnumerator<FileSystemEntry> GetEnumerator()
    {
        yield return this;
        foreach (var entry in Children)
        {
            if (entry is File)
                yield return entry;
            if (entry is Directory subDirectory)
            {
                foreach (var subDirEntry in subDirectory)
                    yield return subDirEntry;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
