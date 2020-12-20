using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Find the IDs of the 4 corner tiles and multiply them all
        /// </summary>
        public long Part1(string[] input)
        {
            Dictionary<long, char[,]> tiles = ParseInput(input);
            (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) = MatchTiles(tiles);
            List<long> corners = FindCorners(edges, edgeLookup);
            return corners.Aggregate(1L, (current, next) => current * next);
        }

        /// <summary>
        /// Stitch all the tiles into a single image (matching tiles by their edges and then flipping/rotating properly)
        /// then search the image for sea monsters and find how many cells contain choppy sea
        /// </summary>
        public long Part2(string[] input)
        {
            Dictionary<long, char[,]> tiles = ParseInput(input);

            char[,] image = Stitch(tiles);

            // rotate/flip the image until you start finding sea monsters
            foreach (char[,] permutation in FlipAndRotatePermutations(image))
            {
                int count = CountMonsters(permutation);

                if (count > 0)
                {
                    return permutation.Search(cell => cell == '#').Count() - (count * 15);
                }
            }

            throw new InvalidOperationException("Didn't find any monsters :(");
        }

        /// <summary>
        /// Parse the input into a lookup of tiles -> their ASCII art representation
        /// </summary>
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

        /// <summary>
        /// Given a collection of edges and which tile(s) match those edges, work out which are corner tiles
        /// </summary>
        /// <param name="edges">Tile to edge lookup</param>
        /// <param name="edgeLookup">Edge to tile(s) lookup</param>
        /// <returns>Four corner tile IDs</returns>
        private static List<long> FindCorners(Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup)
        {
            var corners = new List<long>(4);

            // corner tiles have 2 disconnected edges whereas everything else has either 0 or 1
            foreach (long id in edgeLookup.Keys)
            {
                int count = edgeLookup[id].Sum(edge => edges[edge].Count);

                // 2 for each of the connected edges (this and the connected tile) + 1 for each of the unconnected == 2 + 2 + 1 + 1 == 6
                if (count == 6)
                {
                    corners.Add(id);
                }
            }

            return corners;
        }

        /// <summary>
        /// Given a set of tiles, work out which tiles match each other on an edge
        /// </summary>
        /// <param name="tiles">Tiles keyed by ID</param>
        /// <returns>
        /// - A lookup of tile -> it's edges (encoded as a number)
        /// - A lookup of edge -> tile(s) which use it
        /// </returns>
        private static (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) MatchTiles(Dictionary<long, char[,]> tiles)
        {
            var edges = new Dictionary<int, HashSet<long>>();
            var edgeLookup = new Dictionary<long, int[]>();

            foreach ((long id, char[,] grid) in tiles)
            {
                char[] top = grid.GetRow(0);
                char[] bottom = grid.GetRow(grid.GetLength(0) - 1);
                char[] left = grid.GetColumn(0);
                char[] right = grid.GetColumn(grid.GetLength(1) - 1);

                char[] topFlip = grid.GetRow(0).Reverse().ToArray();
                char[] bottomFlip = grid.GetRow(grid.GetLength(0) - 1).Reverse().ToArray();
                char[] leftFlip = grid.GetColumn(0).Reverse().ToArray();
                char[] rightFlip = grid.GetColumn(grid.GetLength(1) - 1).Reverse().ToArray();

                // use the minimal numeric representation so that the flipped-ness doesn't matter
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

        /// <summary>
        /// Converts a series of dots and hashes to a numeric representation by treating # as 1 and . as 0 then parsing as binary
        /// </summary>
        /// <param name="edge">All values for a given edge</param>
        /// <returns>Numeric representation of the edge values</returns>
        private static int NumericRepresentation(char[] edge)
        {
            int n = 0;
            
            for (int i = 0; i < edge.Length; i++)
            {
                if (edge[i] == '#')
                {
                    n += 1 << i;
                }
            }

            return n;
        }

        /// <summary>
        /// Stitch all the given tiles together into a single large image that can be searched for monsters :)
        /// </summary>
        /// <param name="tiles">Image tiles, which may be in random orientations</param>
        /// <returns>Single stitched-together image created by matching and reorienting the tiles</returns>
        private static char[,] Stitch(Dictionary<long, char[,]> tiles)
        {
            // start at a corner, and walk outwards finding the next tile and so on
            // through all the permutations of flip/rotate until you've got the final image
            (Dictionary<int, HashSet<long>> edges, Dictionary<long, int[]> edgeLookup) = MatchTiles(tiles);
            List<long> corners = FindCorners(edges, edgeLookup);

            var visited = new HashSet<long>();
            var todo = new Queue<(long id, int x, int y)>();
            todo.Enqueue((corners.First(), 0, 0));

            // map of the coordinates of each tile, which is populated with the stripped version as we find them
            // note: it is assumed the first corner is the top-left, otherwise a way of identifying which is which is needed
            var map = new Dictionary<(int x, int y), char[,]>();

            // walk outwards along each edge from the top left corner and find the matching tile with the correct rotation/flip
            while (todo.Any())
            {
                (long current, int currentX, int currentY) = todo.Dequeue();

                if (visited.Contains(current))
                {
                    continue;
                }

                visited.Add(current);

                char[,] currentTile = tiles[current];

                map[(currentX, currentY)] = StripEdges(currentTile);
                
                // find the tiles that this one connects to, as previously computed for part 1
                foreach (HashSet<long> touching in edges.Values.Where(e => e.Contains(current)))
                {
                    foreach (long next in touching)
                    {
                        if (next == current)
                        {
                            // anomaly of how we match the edges
                            continue;
                        }
                        
                        // check current against the flip/rotate permutations of next to work out which way round next needs to be and on which edge
                        var nextPermutations = FlipAndRotatePermutations(tiles[next]).ToArray();
                        
                        foreach (char[,] orientation in nextPermutations)
                        {
                            bool found = false;

                            // check current top against next bottom, current left against next right and so one until one matches
                            if (currentTile.GetRow(currentTile.GetLength(1) - 1).SequenceEqual(orientation.GetRow(0)))
                            {
                                // match on bottom edge of current with top edge of next
                                found = true;
                                todo.Enqueue((next, currentX, currentY + 1));
                            }
                            else if (currentTile.GetColumn(currentTile.GetLength(0) - 1).SequenceEqual(orientation.GetColumn(0)))
                            {
                                // match on right edge of current with left edge of next
                                found = true;
                                todo.Enqueue((next, currentX + 1, currentY));
                            }
                            
                            // note: don't need to do top or left edge of current because of the breadth first search

                            if (found)
                            {
                                // store the current orientation of the tile since that's now the right way round
                                tiles[next] = orientation;
                                break;
                            }
                        }
                    }
                }
            }

            return FlattenMap(map);
        }

        /// <summary>
        /// Strip the outer edges of the tile and just leave the inner cells
        /// </summary>
        /// <param name="input">Input tile</param>
        /// <returns>Same tile but with all edges removed</returns>
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

        /// <summary>
        /// All permutations of flipping and rotating the given tile
        /// </summary>
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

        /// <summary>
        /// Flip a tile horizontally
        /// </summary>
        public static char[,] FlipHorizontal(char[,] input)
        {
            int height = input.GetLength(0);
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

        /// <summary>
        /// Rotate the tile 90 degrees
        /// </summary>
        public static char[,] Rotate90(char[,] input)
        {
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

        /// <summary>
        /// Given a lookup of co-ordinates to tiles, flatten the tiles into a single image
        /// </summary>
        private static char[,] FlattenMap(Dictionary<(int x, int y), char[,]> map)
        {
            int width = map.Keys.Select(k => k.x).Max();
            int height = map.Keys.Select(k => k.y).Max();
            int tileSize = map[(0, 0)].GetLength(0);
            char[,] output = new char[(height + 1) * tileSize, (width + 1) * tileSize];

            for (int y = 0; y <= height; y++)
            for (int x = 0; x <= width; x++)
            {
                var tile = map[(x, y)];

                for (int innerY = 0; innerY < tile.GetLength(0); innerY++)
                for (int innerX = 0; innerX < tile.GetLength(1); innerX++)
                {
                    output[y * tileSize + innerY, x * tileSize + innerX] = tile[innerY, innerX];
                }
            }

            return output;
        }

        /// <summary>
        /// Count how many monsters the image contains
        /// </summary>
        private static int CountMonsters(char[,] image)
        {
            // monster image:
            //
            // ..................#.
            // #....##....##....###
            // .#..#..#..#..#..#...

            const int monsterWidth = 20;
            const int monsterHeight = 3;
            
            int count = 0;
            
            // brute force every possible starting location...
            for (int y = 0; y < image.GetLength(0) - monsterHeight; y++)
            {
                for (int x = 0; x < image.GetLength(1) - monsterWidth; x++)
                {
                    bool found = IsChoppySea(image, x + 18, y)
                              && IsChoppySea(image, x,      y + 1)
                              && IsChoppySea(image, x + 5,  y + 1)
                              && IsChoppySea(image, x + 6,  y + 1)
                              && IsChoppySea(image, x + 11, y + 1)
                              && IsChoppySea(image, x + 12, y + 1)
                              && IsChoppySea(image, x + 17, y + 1)
                              && IsChoppySea(image, x + 18, y + 1)
                              && IsChoppySea(image, x + 19, y + 1)
                              && IsChoppySea(image, x + 1,  y + 2)
                              && IsChoppySea(image, x + 4,  y + 2)
                              && IsChoppySea(image, x + 7,  y + 2)
                              && IsChoppySea(image, x + 10, y + 2)
                              && IsChoppySea(image, x + 13, y + 2)
                              && IsChoppySea(image, x + 16, y + 2);

                    if (found)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Check if a cell is in bounds and, if so, contains choppy sea instead of calm sea
        /// </summary>
        private static bool IsChoppySea(char[,] image, int x, int y)
        {
            if (x >= 0 && x < image.GetLength(1) && y >= 0 && y < image.GetLength(0))
            {
                return image[y, x] == '#';
            }
            
            // assume out of bounds is calm water
            return false;
        }
    }

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
