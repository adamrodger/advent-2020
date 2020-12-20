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

            (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) = GetEdgeLookups(tiles);
            HashSet<long> corners = FindCorners(tiles, edges, edgeLookup);
            return corners.Aggregate(1L, (current, next) => current * next);
        }

        public long Part2(string[] input)
        {
            Dictionary<long, char[,]> tiles = ParseInput(input);

            // stitch the tiles together into an image
            char[,] image = Stitch(tiles);

            // rotate/flip the image until you start finding sea monsters

            return 0;
        }

        private static HashSet<long> FindCorners(Dictionary<long, char[,]> tiles, Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup)
        {
            var corners = new HashSet<long>(); // forward and backwards will show up twice, so use a Set

            IEnumerable<long> outerTiles = edges.Values.Where(e => e.Count == 1) // edge which have nothing else touching them
                                                .Select(e => e.Single())
                                                .Distinct();

            // find the outer tiles that contain two outer edges instead of just one - those are the corners
            foreach (long id in outerTiles)
            {
                int count = edgeLookup[id].Sum(edge => edges[edge].Count);

                Debug.WriteLine($"{id}: {count}");

                if (count == 6) // 2 for each of the connected edges (this and the connected tile) + 1 for each of the unconnected == 2 + 2 + 1 + 1 == 6
                {
                    corners.Add(id);
                }
            }

            return corners;
        }

        private static (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) GetEdgeLookups(Dictionary<long, char[,]> tiles)
        {
            var edges = new Dictionary<int, HashSet<long>>(); // look up of edge pattern to tile(s)
            var edgeLookup = new Dictionary<long, int[]>();

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

                int topNum    = Math.Min(NumericRepresentation(top),    NumericRepresentation(topFlip));
                int bottomNum = Math.Min(NumericRepresentation(bottom), NumericRepresentation(bottomFlip));
                int leftNum   = Math.Min(NumericRepresentation(left),   NumericRepresentation(leftFlip));
                int rightNum  = Math.Min(NumericRepresentation(right),  NumericRepresentation(rightFlip));

                if (!edges.ContainsKey(topNum))
                {
                    edges[topNum] = new HashSet<long>();
                }

                if (!edges.ContainsKey(bottomNum))
                {
                    edges[bottomNum] = new HashSet<long>();
                }

                if (!edges.ContainsKey(leftNum))
                {
                    edges[leftNum] = new HashSet<long>();
                }

                if (!edges.ContainsKey(rightNum))
                {
                    edges[rightNum] = new HashSet<long>();
                }

                edges[topNum].Add(id);
                edges[bottomNum].Add(id);
                edges[leftNum].Add(id);
                edges[rightNum].Add(id);

                edgeLookup[id] = new[] { topNum, bottomNum, leftNum, rightNum};
            }

            return (edges, edgeLookup);
        }

        private static int NumericRepresentation(string rowOrColumn)
        {
            return Convert.ToInt32(rowOrColumn.Replace('#', '1').Replace('.', '0'), 2);
        }

        private char[,] Stitch(Dictionary<long, char[,]> tiles)
        {
            // start at a corner, and walk outwards finding the next tile and so on
            // through all the permutations of flip/rotate until you've got the final image
            (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) = GetEdgeLookups(tiles);
            HashSet<long> corners = FindCorners(tiles, edges, edgeLookup);

            var todo = new Queue<long>();
            todo.Enqueue(corners.First());

            var visited = new HashSet<long>();

            var strippedTiles = new Dictionary<long, char[,]>
            {
                [todo.Peek()] = StripEdges(tiles[todo.Peek()])
            };

            while (todo.Any())
            {
                long current = todo.Dequeue();

                if (visited.Contains(current))
                {
                    continue;
                }

                visited.Add(current);

                char[,] currentTile = tiles[current];
                //Debug.WriteLine($"Checking tile {current}");
                //currentTile.Print();
                //Debug.WriteLine(string.Empty);

                // find the tiles that this one connects to
                foreach (HashSet<long> touching in edges.Values.Where(e => e.Contains(current)))
                {
                    foreach (long next in touching)
                    {
                        if (next == current)
                        {
                            // artifact of how we match the edges
                            continue;
                        }
                        
                        // check current against the flip/rotate permutations of next to work out which way next needs to be and on which edge
                        var nextPermutations = FlipAndRotatePermutations(tiles[next]).ToArray();
                        int i = 0;
                        
                        foreach (char[,] orientation in nextPermutations)
                        {
                            bool found = false;
                            i++;

                            // check current top against next bottom, current left against next right and so one until one matches
                            if (currentTile.GetRow(0).SequenceEqual(orientation.GetRow(orientation.GetLength(1) - 1)))
                            {
                                found = true;
                                Debug.WriteLine($"Tile {current} matches tile {next} on top row with orientation {i}");
                            }
                            else if (currentTile.GetRow(currentTile.GetLength(1) - 1).SequenceEqual(orientation.GetRow(0)))
                            {
                                found = true;
                                Debug.WriteLine($"Tile {current} matches tile {next} on bottom row with orientation {i}");
                            }
                            else if (currentTile.GetColumn(currentTile.GetLength(0) - 1).SequenceEqual(orientation.GetColumn(0)))
                            {
                                found = true;
                                Debug.WriteLine($"Tile {current} matches tile {next} on right column with orientation {i}");
                            }
                            else if (currentTile.GetColumn(0).SequenceEqual(orientation.GetColumn(orientation.GetLength(0) - 1)))
                            {
                                found = true;
                                Debug.WriteLine($"Tile {current} matches tile {next} on left column with orientation {i}");
                            }

                            if (found)
                            {
                                // store the current orientation of the tile since that's now the right way round
                                tiles[next] = orientation;
                                strippedTiles[next] = StripEdges(orientation);
                                
                                break;
                            }
                        }

                        todo.Enqueue(next);
                    }
                }
            }

            throw new NotImplementedException();
        }

        private static char[,] StripEdges(char[,] input)
        {
            var output = new char[input.GetLength(0) - 2, input.GetLength(1) - 2];

            for (int y = 1; y < input.GetLength(0) - 1; y++)
            {
                for (int x = 1; x < input.GetLength(1) - 1; x++)
                {
                    output[y - 1, x - 1] = input[y, x];
                }
            }

            return output;
        }

        private static IEnumerable<char[,]> FlipAndRotatePermutations(char[,] input)
        {
            yield return input;
            
            for (int i = 0; i < 3; i++)
            {
                input = Rotate90(input);
                yield return input;
            }
            
            input = FlipHorizontal(input);
            yield return input;
            
            for (int i = 0; i < 3; i++)
            {
                input = Rotate90(input);
                yield return input;
            };
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
            //int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[height - y - 1, x] = input[y, x];
                }
            }

            return output;
        }

        /*public static char[,] FlipVertical(char[,] input)
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
        }*/

        public static char[,] Rotate90(char[,] input)
        {
            //int height = input.GetLength(0);
            //int width = input.GetLength(1);
            char[,] output = new char[input.GetLength(0), input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                {
                    output[input.GetLength(0) - x - 1, y] = input[y, x];
                }
            }

            return output;
        }

        /*public static char[,] Rotate180(char[,] input)
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
        }*/

        /// <summary>
        /// Makes sure at least one edge of the two are aligned (i.e. match exactly)
        /// </summary>
        /*private static bool AreAligned(char[,] one, char[,] two)
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
        }*/
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
