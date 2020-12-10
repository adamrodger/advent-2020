using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Tests
{
    public class Day10Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day10 solver;

        public Day10Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day10();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day10.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                /*"16",
                "10",
                "15",
                "5",
                "1",
                "11",
                "7",
                "19",
                "6",
                "12",
                "4"*/

                "28",
                "33",
                "18",
                "42",
                "31",
                "14",
                "46",
                "20",
                "48",
                "47",
                "24",
                "23",
                "49",
                "45",
                "19",
                "38",
                "39",
                "11",
                "1",
                "32",
                "25",
                "35",
                "8",
                "17",
                "7",
                "9",
                "4",
                "2",
                "34",
                "10",
                "3"
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 220;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 2738;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 10 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 19208;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            // 274877906944 -- too low
            var expected = 74049191673856;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 10 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
