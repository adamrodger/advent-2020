using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 9
    /// </summary>
    public class Day9
    {
        public long Part1(string[] input)
        {
            long[] numbers = input.Select(long.Parse).ToArray();

            for (int i = 25; i < numbers.Length; i++)
            {
                long[] preamble = numbers[(i - 25)..i];
                IEnumerable<(long x, long y)> pairs = Pairs(preamble);

                if (pairs.All(pair => pair.x + pair.y != numbers[i]))
                {
                    return numbers[i];
                }
            }

            return 0;
        }

        public long Part2(string[] input)
        {
            const long target = 393911906; // from part 1

            var numbers = input.Select(long.Parse).ToArray();

            int i = 0, j = 1;

            // home in on the correct slice by walking the two indices through the numbers
            while (j < numbers.Length)
            {
                if (i == j)
                {
                    j++;
                    continue;
                }

                long[] slice = numbers[i..j];
                long sum = slice.Sum();

                if (sum < target)
                {
                    j++;
                }
                else if (sum > target)
                {
                    i++;
                }
                else
                {
                    return slice.Min() + slice.Max();
                }
            }

            return 0;
        }

        private static IEnumerable<(long x, long y)> Pairs(ICollection<long> numbers)
        {
            foreach (var x in numbers)
            {
                foreach (var y in numbers.Where(n => n != x))
                {
                    yield return (x, y);
                }
            }
        }
    }
}
