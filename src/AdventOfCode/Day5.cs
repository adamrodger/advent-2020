using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 5
    /// </summary>
    public class Day5
    {
        public int Part1(string[] input)
        {
            return input.Select(CalculateSeatId).Max();
        }

        public int Part2(string[] input)
        {
            var ids = input.Select(CalculateSeatId).OrderBy(x => x).ToList();

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == ids[i + 1] - 2)
                {
                    return ids[i] + 1;
                }
            }

            return 0;
        }

        private static int CalculateSeatId(string line)
        {
            int row = Chop(line, 0, 127, 'F');
            int column = Chop(line.Substring(7), 0, 7, 'L');
            int seat = (row * 8) + column;
            return seat;
        }

        private static int Chop(string directions, int min, int max, char lower)
        {
            int delta = (max - min + 1) / 2;

            if (directions[0] == lower)
            {
                max -= delta;
            }
            else
            {
                min += delta;
            }

            if (min == max)
            {
                return min;
            }

            return Chop(directions.Substring(1), min, max, lower);
        }
    }
}
