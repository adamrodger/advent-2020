using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 23
    /// </summary>
    public class Day23
    {
        public string Part1(string[] input)
        {
            int[] cups = input[0].Select(c => int.Parse(c.ToString())).ToArray();
            IList<int> result = Play(cups, 100);

            // collect the order
            var builder = new StringBuilder();
            int current = result[1];

            do
            {
                builder.Append(current);
                current = result[current];
            } while (current != 1);

            return builder.ToString();
        }

        public long Part2(string[] input)
        {
            int[] cups = input[0].Select(c => int.Parse(c.ToString())).ToArray();
            cups = cups.Concat(Enumerable.Range(1, 1_000_000).Skip(cups.Length)).ToArray();
            IList<int> result = Play(cups, 10_000_000);

            // multiply the two nodes after node 1
            int answer1 = result[1];
            long answer2 = result[answer1];
            
            return answer1 * answer2;
        }

        /// <summary>
        /// Play the game of cups
        /// </summary>
        /// <param name="input">Starting cup layout</param>
        /// <param name="iterations">Number of game iterations</param>
        /// <returns>All cups, with a reference to the next cup in each</returns>
        private static IList<int> Play(int[] input, int iterations)
        {
            // create a lookup where index is cup number and value is next cup number
            // note: this is effectively 1-indexed because cups[0] is unused
            int[] cups = new int[input.Length + 1];

            for (int i = 0; i < input.Length - 1; i++)
            {
                int x = input[i];
                cups[x] = input[i + 1];
            }

            // make it cyclic
            cups[input.Last()] = input.First();

            // start at the first cup
            int current = input.First();

            for (int i = 0; i < iterations; i++)
            {
                // 3 cups are chopped out
                int next1 = cups[current];
                int next2 = cups[next1];
                int next3 = cups[next2];
                cups[current] = cups[next3];

                // pick the destination - wrap around if it goes too low and make sure destination isn't in removed cups
                int destination = current;

                do
                {
                    destination--;
                    destination = destination < 1 ? cups.Length - 1 : destination;
                } while (destination == next1 || destination == next2 || destination == next3);

                // insert the 3 cups between the destination and its current pointer
                int temp = cups[destination];
                cups[destination] = next1;
                cups[next3] = temp;

                // advance current
                current = cups[current];
            }

            return cups;
        }
    }
}
