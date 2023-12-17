using AdventOfCode.Helpers;

namespace AdventOfCode.Aoc2022.Day17
{
    /*
     * Created by xkarpi06 on 07.12.2023
     * 
     * Day 17: Pyroclastic Flow
     *
     * time: 5h + 2h
     *
     * Day       Time   Rank  Score       Time   Rank  Score
     *  17       >24h  16399      0          -      -      - 
     */
    public class Day17
    {
        /*
         * There is a very tall, narrow chamber. Large, oddly-shaped rocks are falling into the chamber from above.
         * The rocks don't spin, but they do get pushed around by jets of hot gas coming out of the walls (input).
         * The tall, vertical chamber is exactly seven units wide.
         * After a rock appears, it alternates between being pushed by a jet of hot gas one unit (in the direction
         * indicated by the next symbol in the jet pattern) and then falling one unit down.
         * If a downward movement would have caused a falling rock to move into the floor or an already-fallen rock,
         * the falling rock stops where it is (having landed on something) and a new rock immediately begins falling.
         */
        static void Main(string[] args)
        {
            Run(new ByteChamber4(), reducePeriod: 100_000_000, showResult: true);
            // RunInChamber(new ByteChamber5(), 1_000_000);
        }

        public static void Run(IChamber chamber, long? rocks = null, int? reducePeriod = null, bool showResult = false)
        {
            List<string> input = new InputLoader(AoCYear.AOC_2022).LoadStrings("Day17Input.txt");
            var jetBursts = ParseInput(input);
            Part1(jetBursts, chamber, showResult);
            Part2(jetBursts, chamber, rocks, reducePeriod, showResult);
        }

        public static void RunInChamber(IChamberRunnable chamber, long rocks)
        {
            string jetBursts = new InputLoader(AoCYear.AOC_2022).LoadStrings("Day17Input.txt")[0];
            chamber.RunSimulation(2022, jetBursts);
            chamber.RunSimulation(rocks, jetBursts);
        }

        /*
         * How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
         */
        static void Part1(List<Dir> jetBursts, IChamber chamber, bool showResult = true)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            var rockAmount = 2022;

            var jetCount = 0;

            for (int i = 0; i < rockAmount; i++)
            {
                chamber.SpawnRock(orderedRocks[i % orderedRocks.Count]);
                do
                {
                    chamber.MoveRockWithRules(jetBursts[jetCount++ % jetBursts.Count]);
                } while (chamber.MoveRockWithRules(Dir.DOWN));
                chamber.PutRockToSleep();
            }
            if (showResult) Console.WriteLine($"p1> {chamber.RockTowerHeight}");
        }

        /*
         * How tall will the tower be after 1.000.000.000.000 rocks have stopped?
         *
         * Not finished. Calculation is too slow. 10M rocks per second are being processed, which would require 100K
         * seconds (1 day) to complete.
         */
        static void Part2(List<Dir> jetBursts, IChamber chamber, long? rocks = null, int? reducePeriod = null, bool showResult = true)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            var rockAmount = 1_000_000L; // goal was 1_000_000_000_000L
            var reduceChamberPeriod = 5_000; // (max chamber size 42)
            var printPeriod = 100_000;

            rockAmount = rocks ?? rockAmount;
            reduceChamberPeriod = reducePeriod ?? reduceChamberPeriod;

            int jetCount = 0;
            int rockIndex = 0; // I need Int for list index
            for (long i = 0; i < rockAmount; i++)
            {
                var rock = orderedRocks[rockIndex++ % orderedRocks.Count];
                chamber.SpawnRock(rock);
                do
                {
                    chamber.MoveRockWithRules(direction: jetBursts[jetCount++ % jetBursts.Count]);
                } while (chamber.MoveRockWithRules(direction: Dir.DOWN));
            chamber.PutRockToSleep();

                // reduce chamber size periodically
                if (i % reduceChamberPeriod == 0L)
                {
                    rockIndex %= reduceChamberPeriod;
                    chamber.RemoveUnreachablePartOfChamber();
                    // if (i % printPeriod == 0L)
                    // {
                        // Console.WriteLine($"rocks={i}, chamber-size={chamber.State.Count}, tower-height={chamber.RockTowerHeight}");
                    // }
                }
            }

            if (showResult) Console.WriteLine($"p2> {chamber.RockTowerHeight}");
        }

        private static List<Dir> ParseInput(List<string> input)
        {
            return input[0].Select(c =>
            {
                switch (c)
                {
                    case '<': return Dir.LEFT;
                    case '>': return Dir.RIGHT;
                    default: return Dir.RIGHT;
                }
            }).ToList();
        }
    }
}
