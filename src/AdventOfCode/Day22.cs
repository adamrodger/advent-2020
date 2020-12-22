using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 22
    /// </summary>
    public class Day22
    {
        public long Part1(string[] input)
        {
            var player1 = new Queue<int>(input.Skip(1).TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(int.Parse));
            var player2 = new Queue<int>(input.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(2).Select(int.Parse));

            PlayGame(player1, player2, false);
            return CalculateWinner(player1, player2);
        }
        
        public long Part2(string[] input)
        {
            var player1 = new Queue<int>(input.Skip(1).TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(int.Parse));
            var player2 = new Queue<int>(input.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(2).Select(int.Parse));

            PlayGame(player1, player2, true);
            return CalculateWinner(player1, player2);
        }
        
        private static void PlayGame(Queue<int> player1, Queue<int> player2, bool part2)
        {
            var seen = new HashSet<(string, string)>();

            while (player1.Any() && player2.Any())
            {
                if (part2)
                {
                    // guard against infinite recursion by checking the current card configurations
                    var config1 = string.Join(",", player1);
                    var config2 = string.Join(",", player2);

                    if (seen.Contains((config1, config2)))
                    {
                        player2.Clear(); // player 1 wins
                        return;
                    }

                    seen.Add((config1, config2));
                }

                var card1 = player1.Dequeue();
                var card2 = player2.Dequeue();

                if (part2 && player1.Count >= card1 && player2.Count >= card2)
                {
                    // recurse on copies of decks
                    var copyPlayer1 = new Queue<int>(player1.Take(card1));
                    var copyPlayer2 = new Queue<int>(player2.Take(card2));
                    
                    PlayGame(copyPlayer1, copyPlayer2, true);

                    if (copyPlayer1.Any())
                    {
                        player1.Enqueue(card1);
                        player1.Enqueue(card2);
                    }
                    else
                    {
                        player2.Enqueue(card2);
                        player2.Enqueue(card1);
                    }
                }
                else
                {
                    if (card1 > card2)
                    {
                        player1.Enqueue(card1);
                        player1.Enqueue(card2);
                    }
                    else
                    {
                        player2.Enqueue(card2);
                        player2.Enqueue(card1);
                    }
                }
            }
        }

        private static long CalculateWinner(Queue<int> player1, Queue<int> player2)
        {
            Queue<int> winner = player1.Any() ? player1 : player2;

            long total = 0;

            while (winner.Any())
            {
                total += (winner.Count * winner.Dequeue());
            }

            return total;
        }
    }
}
