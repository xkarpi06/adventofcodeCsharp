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
    class Day17
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
            List<string> input = new InputLoader(AoCYear.AOC_2022).LoadStrings("Day17Input.txt");
            var jetBursts = ParseInput(input);
            // Part1(jetBursts);
            // Part2(jetBursts);
            MeasuredPartExecution(Part2, jetBursts);
        }

        /*
         * How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
         */
        static void Part1(List<Dir> jetBursts)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            var rockAmount = 5;

            IChamber chamber = new SlowChamber();
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
            Console.WriteLine($"p1> {chamber.RockTowerHeight}");
        }


        /*
         * How tall will the tower be after 1.000.000.000.000 rocks have stopped?
         *
         * Not finished. Calculation is too slow. 1M rocks per second are being processed, which would require 1M
         * seconds (10 days) to complete.
         */
        static void Part2(List<Dir> jetBursts)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            // var rockAmount = 1_000_000_000_000L; // goal was 1_000_000_000_000L
            var rockAmount = 1_000_000L; // goal was 1_000_000_000_000L
            var reduceChamberPeriod = 5_000;
            var printPeriod = 100_000;

            IChamber chamber = new SlowChamber();

            int jetCount = 0;
            int rockIndex = 0; // I need Int for list index

            for (long i = 0; i < rockAmount; i++)
            {
                chamber.SpawnRock(orderedRocks[rockIndex++ % orderedRocks.Count]);
                do
                {
                    chamber.MoveRockWithRules(jetBursts[jetCount++ % jetBursts.Count]);
                } while (chamber.MoveRockWithRules(Dir.DOWN));
                chamber.PutRockToSleep();

                // reduce chamber size periodically
                if (i % reduceChamberPeriod == 0L)
                {
                    rockIndex %= reduceChamberPeriod;
                    chamber.RemoveUnreachablePartOfChamber();

                    // if (i % printPeriod == 0L)
                    // {
                    //     Console.WriteLine($"rocks={i}, chamber-size={chamber.State.Count}, tower-height={chamber.RockTowerHeight}");
                    // }
                }
            }

            Console.WriteLine($"p2> {chamber.RockTowerHeight}");
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

        public delegate void Part(List<Dir> jetBursts);

        private static void MeasuredPartExecution(Part p, List<Dir> list)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            p(list);
            watch.Stop();
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
