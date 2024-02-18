using FluentAssertions;
using Day12Solver = AdventOfCode.Y2023.D12.Solver;


namespace UnitTests;

public class Tests
{
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
        var ans = Day12Solver.FindPermutations(line.ToCharArray(), groups);
        ans.Should().Be(expected);
    }

    
    [Theory]
    [InlineData("???.###", new int[] {1,1,3}, 1)]
    [InlineData(".??..??...?##. 1,1,3", new int[] {1,1,3}, 16384)]
    [InlineData("?#?#?#?#?#?#?#?", new int[] {1,3,1,6}, 1)]
    [InlineData("????.#...#...", new int[] {4,1,1}, 16)]
    [InlineData("????.######..#####.", new int[] {1,6,5}, 2500)]
    [InlineData("?###????????", new int[] { 3, 2, 1 }, 506250)]
    //[InlineData("?#???", new int[] { 2 }, 2)]
    //[InlineData("????#", new int[] { 1 }, 1)]
    public void TestFindUnfoldedPermutations(string line, int[] groups, int expected)
    {
        //var line = "?###????????".ToCharArray();
        var ans = Day12Solver.FindUnfoldedPermutations(line.ToCharArray(), groups);
        ans.Should().Be(expected);
    }
    
}