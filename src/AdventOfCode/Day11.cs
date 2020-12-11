using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 11
    /// </summary>
    public class Day11
    {
        public int Part1(string[] input)
        {
            return Simulate(input);
        }

        public int Part2(string[] input)
        {
            return Simulate(input, true);
        }

        private static int Simulate(string[] input, bool part2 = false)
        {
            char[,] grid = input.ToGrid();
            bool changed = true;

            int occupiedLimit = part2 ? 5 : 4;

            while (changed)
            {
                char[,] next = grid.Clone() as char[,];
                changed = false;

                grid.ForEach((x, y, seat) =>
                {
                    if (seat == '.')
                    {
                        return;
                    }

                    var adjacent = !part2 ? grid.Adjacent8(x, y).ToArray()
                                          : Visible(grid, x, y).ToArray();

                    if (seat == 'L' && adjacent.All(c => c != '#'))
                    {
                        next[y, x] = '#';
                        changed = true;
                        return;
                    }

                    if (seat == '#' && adjacent.Count(c => c == '#') >= occupiedLimit)
                    {
                        next[y, x] = 'L';
                        changed = true;
                        return;
                    }
                });

                grid = next;
            }

            return grid.Search(c => c == '#').Count();
        }

        /// <summary>
        /// Get all the seats visible from this location
        /// </summary>
        private static IEnumerable<char> Visible(char[,] grid, int x, int y)
        {
            yield return Walk(grid, x, y, (-1, -1));
            yield return Walk(grid, x, y, (0, -1));
            yield return Walk(grid, x, y, (1, -1));

            yield return Walk(grid, x, y, (-1, 0));
            yield return Walk(grid, x, y, (1, 0));

            yield return Walk(grid, x, y, (-1, 1));
            yield return Walk(grid, x, y, (0, 1));
            yield return Walk(grid, x, y, (1, 1));
        }

        /// <summary>
        /// Walk a delta until you find a seat from a given location
        /// </summary>
        private static char Walk(char[,] grid, int x, int y, (int x, int y) delta)
        {
            x += delta.x;
            y += delta.y;

            // stay in bounds
            while (y < grid.GetLength(0) && x < grid.GetLength(1) && x >= 0 && y >= 0)
            {
                if (grid[y, x] != '.')
                {
                    // found a seat
                    return grid[y, x];
                }

                // keep walking :D
                x += delta.x;
                y += delta.y;
            }

            return '.'; // out of bounds, assume floor
        }
    }
}
