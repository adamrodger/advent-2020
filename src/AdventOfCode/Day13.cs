using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 13
    /// </summary>
    public class Day13
    {
        public int Part1(string[] input)
        {
            int start = int.Parse(input[0]);
            int time = start;
            var buses = input[1].Numbers<int>();

            while (true)
            {
                int leaving = buses.FirstOrDefault(b => time % b == 0);

                if (leaving != default)
                {
                    return (time - start) * leaving;
                }

                time++;
            }
        }

        public long Part2(string[] input)
        {
            string[] strings = input[1].Split(',');
            var buses = new List<int>();
            var diffs = new Dictionary<int, int>();

            for (int i = 0; i < strings.Length; i++)
            {
                string s = strings[i];

                if (s == "x")
                {
                    continue;
                }

                int bus = int.Parse(s);
                buses.Add(bus);
                diffs[bus] = i;
            }

            long time = 0;
            long multiplier = 1;

            // basically, find the LCM between each bus and the previous ones (which satisfies the offset)
            // and advance time by that much until you've done all buses
            foreach (int bus in buses)
            {
                int offset = diffs[bus];

                while ((time + offset) % bus != 0)
                {
                    // jump by the multiplier until you find a time where this bus syncs with the previous ones
                    time += multiplier;
                }

                // include this bus into the jumps to keep everything in sync
                multiplier *= bus;
            }

            return time;
        }
    }
}
