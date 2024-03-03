using FluentAssertions;
using Xunit.Abstractions;
using Day12Solver = AdventOfCode.Y2023.D12.Solver;


namespace UnitTests;

public class Tests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Tests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        var line = "???.###".ToCharArray();
        var ans = Day12Solver.FindPermutations(line, [1,3]);
        ans.Should().Be(3);
    }

    [Theory]
    [InlineData("???.###", new int[] {1,1,3}, 1)]
    [InlineData(".??..??...?##. 1,1,3", new int[] {1,1,3}, 4)]
    [InlineData("?#?#?#?#?#?#?#?", new int[] {1,3,1,6}, 1)]
    [InlineData("????.#...#...", new int[] {4,1,1}, 1)]
    [InlineData("????.######..#####.", new int[] {1,6,5}, 4)]
    [InlineData("?###????????", new int[] { 3, 2, 1 }, 10)]
    [InlineData("?#???", new int[] { 2 }, 2)]
    [InlineData("????#", new int[] { 1 }, 1)]
    public void Test2(string line, int[] groups, int expected)
    {
        //var line = "?###????????".ToCharArray();
        var ans = Day12Solver.FindPermutations1(line, groups);
        ans.Should().Be(expected);
    }

    
    [Theory]
    [InlineData("???.###", new int[] {1,1,3}, 1)]
    [InlineData(".??..??...?##.", new int[] {1,1,3}, 16384)]
    [InlineData("?#?#?#?#?#?#?#?", new int[] {1,3,1,6}, 1)]
    [InlineData("????.#...#...", new int[] {4,1,1}, 16)]
    [InlineData("????.######..#####.", new int[] {1,6,5}, 2500)]
    [InlineData("?###????????", new int[] { 3, 2, 1 }, 506250)]
    [InlineData("?.????..??", new int[] { 1, 3, 1}, 5184)]
    //[InlineData("?#???", new int[] { 2 }, 2)]
    //[InlineData("????#", new int[] { 1 }, 1)]
    public void TestFindUnfoldedPermutations(string line, int[] groups, int expected)
    {
        //var line = "?###????????".ToCharArray();
        var ans = Day12Solver.FindUnfoldedPermutations(line.ToCharArray(), groups);
        ans.Should().Be(expected);
    }

    [Fact]
    public void TestFindAnswers()
    {
        var line = "?.????..??";

        var d = string.Join('?', Enumerable.Range(0, 5).Select(_ => line));
        int[] groups = [1, 3, 1];
        var (newConditions, newGroups) = Day12Solver.Unfold(line.ToCharArray(), groups);
        newConditions.Length.Should().Be(line.Length * 5 + 4);
        newGroups.Length.Should().Be(groups.Length * 5);
        var ans = Day12Solver.FindPermutations1(new string(newConditions), newGroups);
        _testOutputHelper.WriteLine($"Answer: {ans}");
        true.Should().BeTrue();
    }
    
}