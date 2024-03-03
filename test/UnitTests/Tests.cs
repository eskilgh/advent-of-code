using FluentAssertions;
using Xunit.Abstractions;
using Day12Solver = AdventOfCode.Y2023.D12.Solver;


namespace UnitTests;

public class Tests(ITestOutputHelper testOutputHelper)
{

    [Fact]
    public void DummyTest()
    {
        true.Should().BeTrue();
    }
}