using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 18
    /// </summary>
    public class Day18
    {
        public long Part1(string[] input)
        {
            return input.Select(i => Solve(i, true)).Sum();
        }

        public long Part2(string[] input)
        {
            return input.Select(i => Solve(i, false)).Sum();
        }

        private static Regex Parenthesis = new Regex(@"(\([^\(]*?\))", RegexOptions.Singleline | RegexOptions.Compiled); // non-greedy middle match

        private static Regex Either = new Regex(@"(\d+)\s*(\+|\*)\s*(\d+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex Addition = new Regex(@"(\d+)\s*\+\s*(\d+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex Multiplication = new Regex(@"(\d+)\s*\*\s*(\d+)", RegexOptions.Singleline | RegexOptions.Compiled);

        private static long Solve(string line, bool part1)
        {
            // split into sub-problems in parenthesis and solve recursively
            while (line.Contains('('))
            {
                var matches = Parenthesis.Match(line);
                var match = matches.Groups[1].Value;

                long value = Solve(match[1..^1], part1);

                line = line.Replace(match, value.ToString());
            }

            return part1 ? ApplyPart1(line) : ApplyPart2(line);
        }

        /// <summary>
        /// part 1 - just solve left to right
        /// </summary>
        private static long ApplyPart1(string line)
        {
            while (line.Contains('+') || line.Contains('*'))
            {
                Match match = Either.Match(line);

                long a = long.Parse(match.Groups[1].Value);
                string opcode = match.Groups[2].Value;
                long b = long.Parse(match.Groups[3].Value);

                long sum = opcode == "+" ? a + b : a * b;

                line = Either.Replace(line, sum.ToString(), 1);
            }

            return long.Parse(line);
        }

        /// <summary>
        /// part 2 - do addition first then multiplication
        /// </summary>
        private static long ApplyPart2(string line)
        {
            while (line.Contains('+'))
            {
                Match match = Addition.Match(line);

                long a = long.Parse(match.Groups[1].Value);
                long b = long.Parse(match.Groups[2].Value);

                line = Addition.Replace(line, (a + b).ToString(), 1);
            }

            while (line.Contains('*'))
            {
                Match match = Multiplication.Match(line);

                long a = long.Parse(match.Groups[1].Value);
                long b = long.Parse(match.Groups[2].Value);

                line = Multiplication.Replace(line, (a * b).ToString(), 1);
            }

            return long.Parse(line);
        }
    }
}
