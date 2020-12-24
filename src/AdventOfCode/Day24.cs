using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 24
    /// </summary>
    public class Day24
    {
        public int Part1(string[] input)
        {
            Dictionary<(int, int), bool> tiles = SetupTiles(input);
            return tiles.Count(tile => tile.Value);
        }

        public int Part2(string[] input)
        {
            Dictionary<(int, int), bool> tiles = SetupTiles(input);

            for (int i = 0; i < 100; i++)
            {
                tiles = GameOfLife(tiles);
            }
            
            return tiles.Count(tile => tile.Value);
        }

        private static Dictionary<(int, int), bool> SetupTiles(string[] input)
        {
            var tiles = new Dictionary<(int, int), bool>();

            foreach (string line in input)
            {
                // start back at the beginning
                (int x, int y) current = (0, 0);

                for (int i = 0; i < line.Length; i++)
                {
                    char next = line[i];
                    int start = i;

                    if (next == 'n' || next == 's')
                    {
                        // take 2 chars for nw/ne/sw/se
                        i++;
                    }

                    int end = i + 1;
                    string direction = line[start..end];

                    // axial coordinates: https://math.stackexchange.com/a/2643016
                    switch (direction)
                    {
                        case "nw":
                            current = (current.x,     current.y - 1);
                            continue;
                        case "ne":
                            current = (current.x + 1, current.y - 1);
                            continue;
                        case "w":
                            current = (current.x - 1, current.y);
                            continue;
                        case "e":
                            current = (current.x + 1, current.y);
                            continue;
                        case "sw":
                            current = (current.x - 1, current.y + 1);
                            continue;
                        case "se":
                            current = (current.x,     current.y + 1);
                            continue;
                        default:
                            throw new InvalidOperationException($"Invalid move: {direction}");
                    }
                }

                if (!tiles.ContainsKey(current))
                {
                    tiles[current] = false;
                }

                // flip the destination tile
                tiles[current] = !tiles[current];
            }

            return tiles;
        }

        private static Dictionary<(int, int), bool> GameOfLife(Dictionary<(int, int), bool> input)
        {
            // map of how many black tiles each tile is touching
            var map = new Dictionary<(int, int), int>();

            // for each black tile, mark its neighbours as touching it
            foreach (((int x, int y) location, _) in input.Where(kvp => kvp.Value))
            {
                foreach ((int x, int y) adjacent in Adjacent6(location))
                {
                    if (!map.ContainsKey(adjacent))
                    {
                        map[adjacent] = 0;
                    }
                    
                    map[adjacent]++;
                }
            }
            
            // apply the rules now that you know how many black tiles each is touching
            var output = new Dictionary<(int, int), bool>();

            foreach (((int x, int y) location, int count) in map)
            {
                bool isBlack = input.ContainsKey(location) && input[location];

                if (isBlack)
                {
                    // black tiles stay black if they are touching 1 or 2 other black tiles, else flip to white
                    output[location] = count == 1 || count == 2;
                }
                else
                {
                    // white tiles flip to black if they are touching exactly 2 other black tiles
                    output[location] = count == 2;
                }
            }

            return output;
        }
        
        private static IEnumerable<(int x, int y)> Adjacent6((int x, int y) current)
        {
            // axial coordinates: https://math.stackexchange.com/a/2643016

            yield return (current.x,     current.y - 1); // nw
            yield return (current.x + 1, current.y - 1); // ne
            yield return (current.x - 1, current.y);     // w
            yield return (current.x + 1, current.y);     // e
            yield return (current.x - 1, current.y + 1); // sw
            yield return (current.x,     current.y + 1); // se 
        }
    }
}
