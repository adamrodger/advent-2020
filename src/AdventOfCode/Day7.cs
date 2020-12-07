using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 7
    /// </summary>
    public class Day7
    {
        public int Part1(string[] input)
        {
            var rules = ParseRules(input);

            HashSet<string> possible = new HashSet<string>();

            foreach (string start in rules.Keys)
            {
                IEnumerable<string> chain = Chain(start, rules);
                if (chain.Contains("shiny gold"))
                {
                    possible.Add(start);
                }
            }

            return possible.Count - 1; // discount the shiny gold bag itself
        }

        public int Part2(string[] input)
        {
            var rules = ParseRules(input);
            return CountBags(0, "shiny gold", rules) - 1; // discount the shiny gold bag itself
        }

        private static IEnumerable<string> Chain(string current, Dictionary<string, Dictionary<string, int>> bags)
        {
            yield return current;

            foreach (string next in bags[current].Keys)
            {
                foreach (var x in Chain(next, bags))
                {
                    yield return x;
                }
            }
        }

        private static int CountBags(int total, string current, Dictionary<string, Dictionary<string, int>> bags)
        {
            Dictionary<string, int> targets = bags[current];

            int newTotal = total;

            foreach ((string target, int count) in targets)
            {
                newTotal += CountBags(total, target, bags) * count;
            }

            newTotal += 1; // count current bag
            return newTotal;
        }

        private static Dictionary<string, Dictionary<string, int>> ParseRules(string[] input)
        {
            // Examples:
            //     shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
            //     faded blue bags contain no other bags.

            Regex bagRegex = new Regex(@"(\d+) ([a-z ]+)+ bags?", RegexOptions.Singleline | RegexOptions.Compiled);
            Dictionary<string, Dictionary<string, int>> rules = new Dictionary<string, Dictionary<string, int>>();

            foreach (string line in input)
            {
                string[] split = line.Split(new[] { " bags contain " }, StringSplitOptions.None);

                string colour = split[0];
                rules[colour] = new Dictionary<string, int>();

                if (line.EndsWith("no other bags."))
                {
                    continue;
                }

                foreach (string target in split[1].Split(','))
                {
                    Match match = bagRegex.Match(target);
                    int amount = int.Parse(match.Groups[1].Value);
                    string targetColour = match.Groups[2].Value;

                    rules[colour][targetColour] = amount;
                }
            }

            return rules;
        }
    }
}
