using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 14
    /// </summary>
    public class Day14
    {
        public long Part1(string[] input)
        {
            Dictionary<long, long> registers = new Dictionary<long, long>();
            string mask = "";

            foreach (string line in input)
            {
                if (line.StartsWith("mask"))
                {
                    mask = line.Substring(6).Trim();
                    continue;
                }

                var nums = line.Numbers<long>();
                long address = nums[0];

                string valueBits = ApplyMask(mask, nums[1]);
                long value = Convert.ToInt64(valueBits, 2);

                registers[address] = value;
            }

            return registers.Values.Sum();
        }

        public long Part2(string[] input)
        {
            Dictionary<long, long> registers = new Dictionary<long, long>();
            string mask = "";

            foreach (string line in input)
            {
                if (line.StartsWith("mask"))
                {
                    mask = line.Substring(6).Trim();
                    continue;
                }

                var nums = line.Numbers<long>();
                long value = nums[1];

                string addressMask = ApplyMask(mask, nums[0], true);
                ICollection<string> addresses = MultiplyMask(addressMask).ToList();

                foreach (string floating in addresses)
                {
                    long address = Convert.ToInt64(floating, 2);
                    registers[address] = value;
                }
            }

            return registers.Values.Sum();
        }

        private static string ApplyMask(string mask, long value, bool part2 = false)
        {
            string valueBits = Convert.ToString(value, 2).PadLeft(36, '0');

            IEnumerable<char> chars = mask.Zip(valueBits, (m, v) => part2
                                                                ? m != '0' ? m : v      // mask | value
                                                                : m != 'X' ? m : v);    // mask overwrites, X passes value bit through

            return new string(chars.ToArray());
        }

        private static IEnumerable<string> MultiplyMask(string mask)
        {
            if (!mask.Contains('X'))
            {
                return new[] { mask };
            }

            var zero = ReplaceFirst(mask, 'X', '0');
            var one = ReplaceFirst(mask, 'X', '1');

            // branch and recurse
            return MultiplyMask(zero).Concat(MultiplyMask(one));
        }

        private static string ReplaceFirst(string mask, char old, char replacement)
        {
            int index = mask.IndexOf(old);

            var sb = new StringBuilder(mask);
            sb.Remove(index, 1);
            sb.Insert(index, replacement);
            return sb.ToString();
        }
    }
}
