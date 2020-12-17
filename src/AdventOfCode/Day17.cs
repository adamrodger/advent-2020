using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 17
    /// </summary>
    public class Day17
    {
        public int Part1(string[] input)
        {
            var active = new HashSet<(int x, int y, int z)>();

            for (int y = 0; y < input.Length; y++)
            {
                string line = input[y];

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        active.Add((x, y, 0));
                    }
                }
            }

            const int cycles = 6;

            for (int i = 0; i < cycles; i++)
            {
                active = Simulate(active);
            }

            return active.Count;
        }

        public int Part2(string[] input)
        {
            var active = new HashSet<(int x, int y, int z, int w)>();

            for (int y = 0; y < input.Length; y++)
            {
                string line = input[y];

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        active.Add((x, y, 0, 0));
                    }
                }
            }

            const int cycles = 6;

            for (int i = 0; i < cycles; i++)
            {
                active = Simulate(active);
            }

            return active.Count;
        }

        private static HashSet<(int x, int y, int z)> Simulate(HashSet<(int x, int y, int z)> input)
        {
            var output = new HashSet<(int x, int y, int z)>();
            var counts = new Dictionary<(int x, int y, int z), int>();

            // mark each neighbour as touching each active point
            foreach ((int x, int y, int z) neighbour in input.SelectMany(Adjacent26))
            {
                if (!counts.ContainsKey(neighbour))
                {
                    counts[neighbour] = 0;
                }

                counts[neighbour]++;
            }

            foreach ((var point, int count)  in counts)
            {
                if (input.Contains(point) && count == 2)
                {
                    output.Add(point);
                }
                else if (count == 3)
                {
                    output.Add(point);
                }
            }

            return output;
        }

        private static HashSet<(int x, int y, int z, int w)> Simulate(HashSet<(int x, int y, int z, int w)> input)
        {
            var output = new HashSet<(int x, int y, int z, int w)>();
            var counts = new Dictionary<(int x, int y, int z, int w), int>();

            // mark each neighbour as touching each active point
            foreach ((int x, int y, int z, int w) neighbour in input.SelectMany(Adjacent80))
            {
                if (!counts.ContainsKey(neighbour))
                {
                    counts[neighbour] = 0;
                }

                counts[neighbour]++;
            }

            foreach ((var point, int count) in counts)
            {
                if (input.Contains(point) && count == 2)
                {
                    output.Add(point);
                }
                else if (count == 3)
                {
                    output.Add(point);
                }
            }

            return output;
        }

        private static IEnumerable<(int x, int y, int z)> Adjacent26((int x, int y, int z) point)
        {
            var deltas = new[] { -1, 0, 1 };

            foreach (int dx in deltas)
            foreach (int dy in deltas)
            foreach (int dz in deltas)
            {
                if (dx == 0 && dy == 0 && dz == 0)
                {
                    continue;
                }

                yield return (point.x + dx, point.y + dy, point.z + dz);
            }
        }

        private static IEnumerable<(int x, int y, int z, int w)> Adjacent80((int x, int y, int z, int w) point)
        {
            var deltas = new[] { -1, 0, 1 };

            foreach (int dx in deltas)
            foreach (int dy in deltas)
            foreach (int dz in deltas)
            foreach (int dw in deltas)
            {
                if (dx == 0 && dy == 0 && dz == 0 && dw == 0)
                {
                    continue;
                }

                yield return (point.x + dx, point.y + dy, point.z + dz, point.w + dw);
            }
        }
    }
}
