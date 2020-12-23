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
            var cups = new LinkedList<int>(input[0].Select(c => int.Parse(c.ToString())));
            LinkedListNode<int> current = Play(cups, 100);

            // collect the order
            var builder = new StringBuilder();

            while (true)
            {
                current = current.Next ?? cups.First;

                if (current.Value == 1)
                {
                    break;
                }
                
                builder.Append(current.Value);
            }

            return builder.ToString();
        }

        public long Part2(string[] input)
        {
            IEnumerable<int> initial = input[0].Select(c => int.Parse(c.ToString())).ToArray();
            int max = initial.Max();
            var extra = Enumerable.Range(max + 1, 1_000_000 - max);
            
            var cups = new LinkedList<int>(initial.Concat(extra));
            LinkedListNode<int> current = Play(cups, 10_000_000);

            // multiply the two nodes after node 1
            var answer1 = current.Next ?? cups.First;
            var answer2 = answer1.Next ?? cups.First;
            
            return ((long)answer1.Value) * answer2.Value;
        }

        /// <summary>
        /// Play the game of cups
        /// </summary>
        /// <param name="cups">Starting cup layout</param>
        /// <param name="iterations">Number of game iterations</param>
        /// <returns>The cup with value 1 after the game has been played</returns>
        /// <remarks>The cups layout is modified directly during the game</remarks>
        private static LinkedListNode<int> Play(LinkedList<int> cups, int iterations)
        {
            int max = cups.Count;

            // build an index to make the Find() faster
            var lookup = new Dictionary<int, LinkedListNode<int>>();
            var current = cups.First;

            do
            {
                lookup[current.Value] = current;
                current = current.Next;
            } while (current != null);

            // reset current ready for the game
            current = cups.First;

            for (int i = 0; i < iterations; i++)
            {
                // remove 3 from current
                LinkedListNode<int> next1 = current.Next ?? cups.First;
                LinkedListNode<int> next2 = next1.Next ?? cups.First;
                LinkedListNode<int> next3 = next2.Next ?? cups.First;

                cups.Remove(next1);
                cups.Remove(next2);
                cups.Remove(next3);

                // pick the destination - wrap around if it goes too low and make sure destination isn't in removed cups
                int destination = current.Value - 1;
                destination = destination < 1 ? max : destination;

                while (destination == next1.Value || destination == next2.Value || destination == next3.Value)
                {
                    destination--;
                    destination = destination < 1 ? max : destination;
                }

                // insert the 3 cups
                LinkedListNode<int> insert = lookup[destination];
                cups.AddAfter(insert, next1);
                cups.AddAfter(next1,  next2);
                cups.AddAfter(next2,  next3);

                // advance current
                current = current.Next ?? cups.First;
            }

            return lookup[1];
        }
    }
}
