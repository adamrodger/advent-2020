using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 6
    /// </summary>
    public class Day6
    {
        public int Part1(string[] input)
        {
            List<List<string>> groups = ParseGroups(input);

            int sum = 0;

            foreach (List<string> @group in groups)
            {
                var unique = new HashSet<char>(@group.SelectMany(c => c));
                sum += unique.Count;
            }

            return sum;
        }

        public int Part2(string[] input)
        {
            List<List<string>> groups = ParseGroups(input);

            int sum = 0;

            foreach (List<string> @group in groups)
            {
                var unique = new HashSet<char>(@group.SelectMany(c => c));
                var common = unique.Count(u => @group.All(g => g.Contains(u)));
                sum += common;
            }

            return sum;
        }

        private static List<List<string>> ParseGroups(string[] input)
        {
            var group = new List<string>();
            var groups = new List<List<string>> { group };

            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    group = new List<string>();
                    groups.Add(group);
                    continue;
                }

                group.Add(line);
            }

            return groups;
        }
    }
}
