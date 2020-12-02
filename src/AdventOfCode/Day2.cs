using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 2
    /// </summary>
    public class Day2
    {
        private static readonly Regex PolicyRegex = new Regex(@"(\d+)-(\d+) (\w): (\w+)", RegexOptions.Compiled | RegexOptions.Singleline);

        public int Part1(string[] input)
        {
            int valid = 0;

            foreach (string line in input)
            {
                (int min, int max, char expected, string password) = ParsePasswordPolicy(line);

                int count = password.Count(c => c == expected);

                if (count >= min && count <= max)
                {
                    valid++;
                }
            }

            return valid;
        }

        public int Part2(string[] input)
        {
            int valid = 0;

            foreach (string line in input)
            {
                (int min, int max, char expected, string password) = ParsePasswordPolicy(line);

                if (password[min - 1] == expected ^ password[max - 1] == expected)
                {
                    valid++;
                }
            }

            return valid;
        }

        private static (int min, int max, char expected, string password) ParsePasswordPolicy(string line)
        {
            Match matches = PolicyRegex.Match(line);

            return (int.Parse(matches.Groups[1].Value),
                    int.Parse(matches.Groups[2].Value),
                    char.Parse(matches.Groups[3].Value),
                    matches.Groups[4].Value);
        }
    }
}
