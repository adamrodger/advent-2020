using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 1
    /// </summary>
    public class Day1
    {
        public int Part1(string[] input)
        {
            var nums = input.Select(int.Parse).ToList();

            return (from x in nums
                    from y in nums
                    where x + y == 2020
                    select x * y).FirstOrDefault();
        }

        public int Part2(string[] input)
        {
            var nums = input.Select(int.Parse).ToList();

            return (from x in nums
                    from y in nums
                    from z in nums
                    where x + y + z == 2020
                    select x * y * z).FirstOrDefault();
        }
    }
}
