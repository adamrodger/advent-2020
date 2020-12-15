using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 15
    /// </summary>
    public class Day15
    {
        public int Part1(string[] input)
        {
            const int target = 2020;
            return Simulate(input, target);
        }

        public int Part2(string[] input)
        {
            const int target = 30_000_000;
            return Simulate(input, target);
        }

        private static int Simulate(string[] input, int target)
        {
            int[] source = input[0].Split(',').Select(int.Parse).ToArray();
            int turn = 0;

            // lookup of number to the last turn where it was seen
            Dictionary<int,int> lookup = new Dictionary<int, int>();

            foreach (int n in source)
            {
                lookup[n] = turn++;
            }

            int current = source.Last();
            int next = 0;

            for (; turn < target; turn++)
            {
                current = next;
                next = lookup.ContainsKey(current)
                               ? turn - lookup[current]
                               : 0;

                lookup[current] = turn;
            }

            return current;
        }
    }
}
