using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 21
    /// </summary>
    public class Day21
    {
        public int Part1(string[] input)
        {
            Dictionary<string, HashSet<string>> possible = CalculatePossibleIngredients(input);

            var assigned = possible.SelectMany(p => p.Value).ToHashSet();
            int count = 0;
            
            foreach (string line in input)
            {
                string[] split = line.Split(" (contains ");
                string[] ingredients = split[0].Split(' ');

                count += ingredients.Count(i => !assigned.Contains(i));
            }

            return count;
        }

        public string Part2(string[] input)
        {
            Dictionary<string, HashSet<string>> possible = CalculatePossibleIngredients(input);

            while (possible.Values.Any(v => v.Count > 1))
            {
                HashSet<string> definite = possible.Where(kvp => kvp.Value.Count == 1).SelectMany(kvp => kvp.Value).ToHashSet();

                foreach (HashSet<string> set in possible.Values.Where(p => p.Count > 1))
                {
                    set.RemoveWhere(m => definite.Contains(m));
                }
            }

            var keys = possible.Keys.OrderBy(p => p).ToArray();
            var ingredients = keys.Select(k => possible[k].First());
            return string.Join(",", ingredients);
        }

        /// <summary>
        /// Lookup of allergen to possible ingredients containing it
        /// </summary>
        private static Dictionary<string, HashSet<string>> CalculatePossibleIngredients(string[] input)
        {
            // lookup allergen to possible ingredients
            var possible = new Dictionary<string, HashSet<string>>();

            foreach (string line in input)
            {
                // mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
                string[] split = line.Split(" (contains ");
                string[] ingredients = split[0].Split(' ');
                string[] allergens = split[1][0..^1].Split(", ");

                foreach (string allergen in allergens)
                {
                    if (!possible.ContainsKey(allergen))
                    {
                        possible[allergen] = new HashSet<string>(ingredients);
                        continue;
                    }

                    possible[allergen] = possible[allergen].Intersect(ingredients).ToHashSet();
                }
            }

            return possible;
        }
    }
}
