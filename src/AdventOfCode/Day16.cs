using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 16
    /// </summary>
    public class Day16
    {
        public int Part1(string[] input)
        {
            (Dictionary<string, int[]> rules, _, List<int[]> others) = ParseInput(input);

            return (from other in others
                    from field in other
                    where !rules.Values.Any(rule => (field >= rule[0] && field <= rule[1])
                                                 || (field >= rule[2] && field <= rule[3]))
                    select field).Sum();
        }

        public long Part2(string[] input)
        {
            (Dictionary<string, int[]> rules, int[] ticket, List<int[]> others) = ParseInput(input);

            List<int[]> valid = others.Where(other => IsValid(other, rules)).ToList();

            // lookup all values across all valid tickets for each field
            Dictionary<int, int[]> values = new Dictionary<int, int[]>();

            for (int field = 0; field < ticket.Length; field++)
            {
                values[field] = new[] { ticket[field] }.Concat(valid.Select(v => v[field])).ToArray();
            }

            // work out which rule goes with which field
            Dictionary<int, string> mapping = new Dictionary<int, string>();
            HashSet<string> remainingRules = rules.Keys.ToHashSet();
            HashSet<int> remainingFields = Enumerable.Range(0, 20).ToHashSet();

            while (remainingRules.Count > 0)
            {
                foreach ((string name, int[] rule) in rules.Where(r => remainingRules.Contains(r.Key)))
                {
                    // check which remaining field(s) could match this rule - hopefully only one!
                    int[] matches = remainingFields.Where(field => values[field].All(v => (v >= rule[0] && v <= rule[1])
                                                                                       || (v >= rule[2] && v <= rule[3])))
                                                   .ToArray();

                    if (matches.Length == 1)
                    {
                        // this is the only rule that can match this field
                        int field = matches.Single();
                        mapping[field] = name;

                        remainingRules.Remove(name);
                        remainingFields.Remove(field);
                    }
                }
            }

            return mapping.Where(kvp => kvp.Value.StartsWith("departure"))
                          .Aggregate(1L, (total, kvp) => total * ticket[kvp.Key]);
        }

        private static bool IsValid(int[] ticket, Dictionary<string, int[]> rules)
        {
            return ticket.All(field => rules.Any(rule => (field >= rule.Value[0] && field <= rule.Value[1])
                                                      || (field >= rule.Value[2] && field <= rule.Value[3])));
        }

        private static (Dictionary<string, int[]> rules, int[] ticket, List<int[]> others) ParseInput(string[] input)
        {
            var rules = new Dictionary<string, int[]>();
            int[] ticket = null;
            var others = new List<int[]>();

            int stage = 0;

            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    stage++;
                    continue;
                }

                if (line.Contains("your ticket") || line.Contains("nearby ticket"))
                {
                    continue;
                }

                switch (stage)
                {
                    case 0:
                    {
                        string[] split = line.Split(':');
                        string column = split[0];

                        var match = Regex.Match(split[1], "(\\d+)-(\\d+) or (\\d+)-(\\d+)");
                        int[] numbers = match.Groups.Skip(1).Select(g => g.Value).Select(int.Parse).ToArray();

                        rules[column] = numbers;
                        break;
                    }
                    case 1:
                        ticket = line.Split(',').Select(int.Parse).ToArray();
                        break;
                    case 2:
                        others.Add(line.Split(',').Select(int.Parse).ToArray());
                        break;
                }
            }

            return (rules, ticket, others);
        }
    }
}
