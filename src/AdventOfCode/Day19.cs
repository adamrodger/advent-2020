using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 19
    /// </summary>
    public class Day19
    {
        private static readonly Regex Digit = new Regex(@"(\d+)", RegexOptions.Singleline | RegexOptions.Compiled);

        public int Part1(string[] input) => CountMatches(input, false);
        public int Part2(string[] input) => CountMatches(input, true);

        /// <summary>
        /// Convert the rules from the input into a regex and count how many times the examples match
        /// </summary>
        private static int CountMatches(string[] input, bool part2)
        {
            Dictionary<string, string> rules = ParseRules(input);

            string pattern = BuildRegex("0", rules, part2);

            if (part2)
            {
                // daft hack because you can't use actual numbers in the regex or they get replaced with sub-rules
                pattern = pattern.Replace("one", "1")
                                 .Replace("two", "2")
                                 .Replace("three", "3")
                                 .Replace("four", "4")
                                 .Replace("five", "5")
                                 .Replace("six", "6")
                                 .Replace("seven", "7")
                                 .Replace("eight", "8")
                                 .Replace("nine", "9");
            }

            Regex regex = new Regex($"^{pattern}$",
                                    RegexOptions.Singleline
                                  | RegexOptions.Compiled
                                  | RegexOptions.ExplicitCapture
                                  | RegexOptions.IgnorePatternWhitespace);

            return input.SkipWhile(line => !string.IsNullOrWhiteSpace(line)) // skip the rules definitions
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Count(i => regex.IsMatch(i));
        }

        /// <summary>
        /// Parse the rules portion of the input
        /// </summary>
        private static Dictionary<string, string> ParseRules(string[] input)
        {
            var rules = new Dictionary<string, string>();

            foreach (string line in input.TakeWhile(l => !string.IsNullOrWhiteSpace(l)))
            {
                var parts = line.Split(": ");
                string key = parts[0];
                string value = parts[1].Replace("\"", string.Empty);

                if (value.Contains('|'))
                {
                    value = $"({value})";
                }

                rules[key] = value;
            }

            return rules;
        }

        /// <summary>
        /// Recursively replace sub-rule references with the actual text to convert into a regex string
        /// </summary>
        private static string BuildRegex(string key, Dictionary<string, string> rules, bool part2)
        {
            if (part2 && key == "8")
            {
                // 8 = 42+
                return BuildRegex("42", rules, true) + "+";
            }

            if (part2 && key == "11")
            {
                // 11 = (42{n} 31{n}) so must be the same sequence length
                var a = BuildRegex("42", rules, true);
                var b = BuildRegex("31", rules, true);

                // can't use numbers or they'll be converted to sub-rules by the recursion
                string hack = "("
                     + "(42{one} 31{one}) | "
                     + "(42{two} 31{two}) | "
                     + "(42{three} 31{three}) | "
                     + "(42{four} 31{four}) | "
                     + "(42{five} 31{five}) | "
                     + "(42{six} 31{six}) | "
                     + "(42{seven} 31{seven}) | "
                     + "(42{eight} 31{eight}) | "
                     + "(42{nine} 31{nine})"
                     + ")";

                return hack.Replace("42", a).Replace("31", b);
            }

            Match match = Digit.Match(rules[key]);

            while (match.Success)
            {
                string replacement = BuildRegex(match.Groups[1].Value, rules, part2);

                rules[key] = Digit.Replace(rules[key], replacement, 1);

                match = Digit.Match(rules[key]);
            }

            return rules[key];
        }
    }
}
