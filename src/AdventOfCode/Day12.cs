using System;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 12
    /// </summary>
    public class Day12
    {
        public int Part1(string[] input)
        {
            (int x, int y) position = (0, 0);
            Bearing bearing = Bearing.East;

            foreach (string instruction in input)
            {
                var code = instruction[0];
                var number = int.Parse(instruction.Substring(1));

                if (code == 'L' || code == 'R')
                {
                    number /= 90;
                }

                switch (code)
                {
                    case 'N':
                        position = position.Move(Bearing.North, number);
                        break;
                    case 'S':
                        position = position.Move(Bearing.South, number);
                        break;
                    case 'E':
                        position = position.Move(Bearing.East, number);
                        break;
                    case 'W':
                        position = position.Move(Bearing.West, number);
                        break;
                    case 'F':
                        position = position.Move(bearing, number);
                        break;
                    case 'L':
                        for (int i = 0; i < number; i++)
                        {
                            bearing = bearing.Turn(TurnDirection.Left);
                        }

                        break;
                    case 'R':
                        for (int i = 0; i < number; i++)
                        {
                            bearing = bearing.Turn(TurnDirection.Right);
                        }

                        break;
                }
            }

            return Math.Abs(position.x) + Math.Abs(position.y);
        }

        public int Part2(string[] input)
        {
            (int x, int y) position = (0, 0);
            (int x, int y) waypoint = (10, 1);

            foreach (string instruction in input)
            {
                var code = instruction[0];
                var number = int.Parse(instruction.Substring(1));

                if (code == 'L' || code == 'R')
                {
                    number /= 90;
                }

                switch (code)
                {
                    // move the waypoint
                    case 'N':
                        waypoint = waypoint.Move(Bearing.North, number);
                        break;
                    case 'S':
                        waypoint = waypoint.Move(Bearing.South, number);
                        break;
                    case 'E':
                        waypoint = waypoint.Move(Bearing.East, number);
                        break;
                    case 'W':
                        waypoint = waypoint.Move(Bearing.West, number);
                        break;

                    case 'F':
                        // move on the waypoint's heading
                        position = (position.x + (waypoint.x * number), position.y + (waypoint.y * number));

                        break;

                    // rotate the waypoint
                    case 'L':
                        for (int i = 0; i < number; i++)
                        {
                            waypoint = (waypoint.y * -1, waypoint.x);
                        }

                        break;
                    case 'R':
                        for (int i = 0; i < number; i++)
                        {
                            waypoint = (waypoint.y, waypoint.x * -1);
                        }

                        break;
                }
            }

            return Math.Abs(position.x) + Math.Abs(position.y);
        }
    }
}
