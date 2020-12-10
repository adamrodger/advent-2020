using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 10
    /// </summary>
    public class Day10
    {
        public int Part1(string[] input)
        {
            var numbers = input.Select(int.Parse).ToList();

            int max = numbers.Max();
            int oneDiff = 0, threeDiff = 1;
            int current = 0;

            while (current < max)
            {
                var next = numbers.Where(n => n <= current + 3).Min();
                numbers.Remove(next);

                if (next == current + 1)
                {
                    oneDiff++;
                }
                else if (next == current + 3)
                {
                    threeDiff++;
                }

                current = next;
            }

            return oneDiff * threeDiff;
        }

        public long Part2(string[] input)
        {
            var numbers = input.Select(int.Parse).OrderByDescending(n => n).ToList();
            numbers.Add(0);
            int max = numbers.Max();
            int target = max + 3;

            var paths = new Dictionary<int, long> { { target, 1 } };

            // walk backwards (instead of forwards!) through the numbers and work out how many different ways there were to get to each one
            foreach (int current in numbers)
            {
                paths.TryGetValue(current + 1, out long oneDiff);
                paths.TryGetValue(current + 2, out long twoDiff);
                paths.TryGetValue(current + 3, out long threeDiff);
                paths[current] = oneDiff + twoDiff + threeDiff;
            }

            return paths[0];
        }
    }
}
