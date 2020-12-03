using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 3
    /// </summary>
    public class Day3
    {
        public long Part1(string[] input)
        {
            char[,] grid = input.ToGrid();
            long trees = TreeSearch(grid, 3, 1);
            return trees;
        }

        public long Part2(string[] input)
        {
            char[,] grid = input.ToGrid();
            return TreeSearch(grid, 1, 1) *
                   TreeSearch(grid, 3, 1) *
                   TreeSearch(grid, 5, 1) *
                   TreeSearch(grid, 7, 1) *
                   TreeSearch(grid, 1, 2);
        }

        private static long TreeSearch(char[,] grid, int dx, int dy)
        {
            long trees = 0;
            int x = 0;

            for (int y = 0; y < grid.GetLength(0); y += dy)
            {
                if (grid[y, x] == '#')
                {
                    trees++;
                }

                x = (x + dx) % grid.GetLength(1);
            }

            return trees;
        }
    }
}
