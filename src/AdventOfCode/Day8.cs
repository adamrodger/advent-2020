using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 8
    /// </summary>
    public class Day8
    {
        public int Part1(string[] input)
        {
            int acc = Execute(input);
            return acc;
        }

        public int Part2(string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                string line = input[i];

                if (line.StartsWith("acc"))
                {
                    continue;
                }

                try
                {
                    // flip nop to jmp and vice versa
                    var clone = input.ToList();
                    if (clone[i].StartsWith("nop"))
                    {
                        clone[i] = clone[i].Replace("nop", "jmp");
                    }
                    else
                    {
                        clone[i] = clone[i].Replace("jmp", "nop");
                    }

                    return Execute(clone.ToArray(), true);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }

            return 0;
        }

        private static int Execute(string[] input, bool part2 = false)
        {
            int acc = 0;
            int index = 0;

            HashSet<int> seen = new HashSet<int>();

            while (!seen.Contains(index))
            {
                if (index >= input.Length)
                {
                    return acc;
                }

                seen.Add(index);
                string line = input[index];

                if (line.StartsWith("nop"))
                {
                    index++;
                    continue;
                }

                int arg = int.Parse(line.Split(' ')[1]);

                if (line.StartsWith("acc"))
                {
                    acc += arg;
                    index++;
                    continue;
                }

                if (line.StartsWith("jmp"))
                {
                    index += arg;
                    continue;
                }
            }

            if (part2)
            {
                throw new InvalidOperationException("infinite loop");
            }

            return acc;
        }
    }
}
