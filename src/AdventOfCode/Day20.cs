using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Utilities;
using MoreLinq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 20
    /// </summary>
    public class Day20
    {
        public long Part1(string[] input)
        {
            Dictionary<long, char[,]> tiles = ParseInput(input);

            // 144 tiles, 12x12 arrangement
            
            // get the outer edges of each tile (forward and backwards to allow for flipping/rotating)
            // and find the tiles where exactly 2 edges match another tile - those are the corners

            // look up of edge pattern to tile(s)
            var edges = new Dictionary<string, HashSet<long>>();
            var edgeLookup = new Dictionary<long, string[]>();
            var corners = new HashSet<long>(); // forward and backwards will show up twice, so use a Set

            foreach ((long id, char[,] grid) in tiles)
            {
                string top = new string(grid.GetRow(0));
                string bottom = new string(grid.GetRow(grid.GetLength(0) - 1));
                string left = new string(grid.GetColumn(0));
                string right = new string(grid.GetColumn(grid.GetLength(1) - 1));

                string topFlip = new string(grid.GetRow(0).Reverse().ToArray());
                string bottomFlip = new string(grid.GetRow(grid.GetLength(0) - 1).Reverse().ToArray());
                string leftFlip = new string(grid.GetColumn(0).Reverse().ToArray());
                string rightFlip = new string(grid.GetColumn(grid.GetLength(1) - 1).Reverse().ToArray());

                if (!edges.ContainsKey(top))
                {
                    edges[top] = new HashSet<long>();
                }
                if (!edges.ContainsKey(bottom))
                {
                    edges[bottom] = new HashSet<long>();
                }
                if (!edges.ContainsKey(left))
                {
                    edges[left] = new HashSet<long>();
                }
                if (!edges.ContainsKey(right))
                {
                    edges[right] = new HashSet<long>();
                }
                if (!edges.ContainsKey(topFlip))
                {
                    edges[topFlip] = new HashSet<long>();
                }
                if (!edges.ContainsKey(bottomFlip))
                {
                    edges[bottomFlip] = new HashSet<long>();
                }
                if (!edges.ContainsKey(leftFlip))
                {
                    edges[leftFlip] = new HashSet<long>();
                }
                if (!edges.ContainsKey(rightFlip))
                {
                    edges[rightFlip] = new HashSet<long>();
                }

                edges[top].Add(id);
                edges[bottom].Add(id);
                edges[left].Add(id);
                edges[right].Add(id);
                edges[topFlip].Add(id);
                edges[bottomFlip].Add(id);
                edges[leftFlip].Add(id);
                edges[rightFlip].Add(id);

                edgeLookup[id] = new[] {top, bottom, left, right, topFlip, bottomFlip, leftFlip, rightFlip};
            }

            /*foreach ((long id, string[] tileEdges) in edgeLookup)
            {
                int inner = 0, outer = 0;
                
                foreach (string edge in tileEdges)
                {
                    if (edges[edge].Count == 2)
                    {
                        inner++;
                    }
                    else if (edges[edge].Count == 1)
                    {
                        outer++;
                    }
                }

                if (inner == 2)
                {
                    corners.Add(id);
                }
            }*/

            IEnumerable<long> outerTiles = edges.Values.Where(e => e.Count == 1)
                                                .Select(e => e.Single())
                                                .Distinct();

            // find the outer tiles that contain two outer edges instead of just one - those are the corners
            foreach (long id in outerTiles)
            {
                int count = 0;
                
                foreach (string edge in edgeLookup[id])
                {
                    count += edges[edge].Count;
                }
                
                Debug.WriteLine($"{id}: {count}");

                if (count == 12)
                {
                    corners.Add(id);
                }
            }
            
            return corners.Aggregate(1L, (current, next) => current * next);
        }

        public long Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }
        
        private static Dictionary<long, char[,]> ParseInput(string[] input)
        {
            var tiles = new Dictionary<long, char[,]>();
            
            foreach (IEnumerable<string> tile in input.Split(string.Empty))
            {
                string id = tile.First()[5..^1];
                char[,] grid = tile.Skip(1).ToArray().ToGrid();

                tiles[long.Parse(id)] = grid;
            }

            return tiles;
        }

        public static char[,] FlipHorizontal(char[,] input)
        {
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[height - y, x] = input[y, x];
                }
            }

            return output;
        }

        public static char[,] FlipVertical(char[,] input)
        {
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[y, width - x] = input[y, x];
                }
            }

            return output;
        }

        public static char[,] Rotate90(char[,] input)
        {
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[-x, y] = input[y, x];
                }
            }

            return output;
        }

        public static char[,] Rotate180(char[,] input)
        {
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[height - y, width - x] = input[y, x];
                }
            }

            return output;
        }

        public static char[,] Rotate270(char[,] input)
        {
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[x, -y] = input[y, x];
                }
            }

            return output;
        }

        /// <summary>
        /// Makes sure at least one edge of the two are aligned (i.e. match exactly)
        /// </summary>
        private static bool AreAligned(char[,] one, char[,] two)
        {
            // top one, bottom two
            string oneTop = new string(one.GetRow(0));
            string oneBottom = new string(one.GetRow(one.GetLength(0) - 1));
            string oneLeft = new string(one.GetColumn(0));
            string oneRight = new string(one.GetColumn(one.GetLength(1) - 1));

            string twoTop = new string(two.GetRow(0));
            string twoBottom = new string(two.GetRow(two.GetLength(0) - 1));
            string twoLeft = new string(two.GetColumn(0));
            string twoRight = new string(two.GetColumn(two.GetLength(1) - 1));

            return oneTop == twoBottom || oneLeft == twoRight || oneRight == twoLeft || oneBottom == twoTop;
        }
    }

    // Mostly copied from https://stackoverflow.com/a/51241629
    public static class ArrayExtensions
    {
        public static char[] GetColumn(this char[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                             .Select(x => matrix[x, columnNumber])
                             .ToArray();
        }

        public static char[] GetRow(this char[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                             .Select(x => matrix[rowNumber, x])
                             .ToArray();
        }
    }
}
