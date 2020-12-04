using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 4
    /// </summary>
    public class Day4
    {
        private static readonly string[] RequiredFields = { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid", };
        private static readonly string[] EyeColours = { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
        private static readonly Regex HeightCm = new Regex(@"\d+cm", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex HeightIn = new Regex(@"\d+in", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex HairColour = new Regex("^#[0-9a-f]{6}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex PassportId = new Regex(@"^\d{9}$", RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Dictionary<string, Predicate<string>> Validation = new Dictionary<string, Predicate<string>>
        {
            ["byr"] = value => value.Length == 4 && (int.Parse(value) >= 1920) && (int.Parse(value) <= 2002),
            ["iyr"] = value => value.Length == 4 && (int.Parse(value) >= 2010) && (int.Parse(value) <= 2020),
            ["eyr"] = value => value.Length == 4 && (int.Parse(value) >= 2020) && (int.Parse(value) <= 2030),
            ["hgt"] = value =>
            {
                if (HeightCm.IsMatch(value))
                {
                    int cm = int.Parse(value.Replace("cm", ""));
                    return cm >= 150 && cm <= 193;
                }

                if (HeightIn.IsMatch(value))
                {
                    int cm = int.Parse(value.Replace("in", ""));
                    return cm >= 59 && cm <= 76;
                }

                return false;
            },
            ["hcl"] = value => HairColour.IsMatch(value),
            ["ecl"] = value => EyeColours.Contains(value),
            ["pid"] = value => PassportId.IsMatch(value),
            ["cid"] = value => true
        };

        public int Part1(string[] input)
        {
            ICollection<Dictionary<string, string>> passports = ParsePassports(input);

            return passports.Count(passport => RequiredFields.All(passport.ContainsKey));
        }

        public int Part2(string[] input)
        {
            ICollection<Dictionary<string, string>> passports = ParsePassports(input);

            return passports.Where(passport => RequiredFields.All(passport.ContainsKey))
                            .Count(passport => passport.All(kvp => Validation[kvp.Key](kvp.Value)));
        }

        private static ICollection<Dictionary<string, string>> ParsePassports(string[] input)
        {
            Dictionary<string, string> passport = new Dictionary<string, string>();
            List<Dictionary<string, string>> passports = new List<Dictionary<string, string>> { passport };

            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    passport = new Dictionary<string, string>();
                    passports.Add(passport);
                    continue;
                }

                foreach (string field in line.Split(' '))
                {
                    string[] parts = field.Split(':');
                    passport[parts[0]] = parts[1];
                }
            }

            return passports;
        }
    }
}
