namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 25
    /// </summary>
    public class Day25
    {
        public long Part1(string[] input)
        {
            long door = int.Parse(input[0]);
            long card = int.Parse(input[1]);

            long value = 1;
            int loop = 0;
            
            while (value != door)
            {
                value = (value * 7) % 20201227;
                loop++;
            }

            value = 1;

            for (int i = 0; i < loop; i++)
            {
                value = (value * card) % 20201227;
            }

            return value;
        }
    }
}
