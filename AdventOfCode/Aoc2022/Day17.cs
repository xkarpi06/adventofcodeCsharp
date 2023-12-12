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
            Part1(jetBursts);
            Part2(jetBursts);
        }

        /*
         * How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
         */
        static void Part1(List<Dir> jetBursts)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            var rockAmount = 5;

            var chamber = new TallNarrowChamber();
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
            var rockAmount = 600_000L; // goal was 1_000_000_000_000L
            var reduceChamberPeriod = 5_000;
            var printPeriod = 100_000;

            var chamber = new TallNarrowChamber();

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

                    if (i % printPeriod == 0L)
                    {
                        Console.WriteLine($"rocks={i}, chamber-size={chamber.State.Count}, tower-height={chamber.RockTowerHeight}");
                    }
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
    }

    /**
     * There are falling rocks (one at a time) inside a tall chamber and a tower made of stationary fallen rocks.
     * Falling rock is pushed by hot jets to left and right while falling.
     */
    public class TallNarrowChamber
    {
        private const int OFFSET_LEFT = 2;
        private const int OFFSET_BOTTOM = 3;
        private const int WIDTH = 7;

        public LinkedList<List<bool>> State { get; private set; }
        public CurrentRock? CurrentRock { get; set; }
        public long CutChamberFloors { get; set; }

        public long RockTowerHeight =>
            State.Count - State.TakeWhile(row => row.All(cell => !cell)).Count() + CutChamberFloors;    

        public TallNarrowChamber()
        {
            State = new LinkedList<List<bool>>();
            CurrentRock = null;
            CutChamberFloors = 0;
        }

        /*
         * Each rock appears so that its left edge is two units away from the left wall and its bottom
         * edge is three units above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock)
        {
            var topVoidRows = State.TakeWhile(row => row.All(cell => !cell)).Count();
            var voidRowsNeeded = OFFSET_BOTTOM + rock.GetHeight();
            var rowsMissing = voidRowsNeeded - topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                State.AddFirst(Enumerable.Repeat(false, WIDTH).ToList());
            }

            CurrentRock = new CurrentRock(rock, new XY(OFFSET_LEFT, rowsMissing > 0 ? 0 : 0 - rowsMissing));
        }

        /*
         * Move rock if it can be moved (tower interference & walls prevent movement)
         * returns false if rock can't fall anymore
         */
        public bool MoveRockWithRules(Dir direction)
        {
            // check if rock can move (checks only walls and bottom)
            if (ChamberWallPreventsMove(direction)) return direction != Dir.DOWN;
            // move rock
            MoveRock(direction);
            // check tower interference & undo move if needed
            if (!CurrentRockOverlaysTower()) return true;
            MoveRock(direction.Opposite());
            return direction != Dir.DOWN;
        }

        /*
         * moves rock without checking interference (walls prevent movement)
         */
        private void MoveRock(Dir direction)
        {
            if (CurrentRock == null || ChamberWallPreventsMove(direction)) return;
            XY old = CurrentRock.Offset;
            var offsetNew = direction switch
            {
                Dir.LEFT => new XY(old.X - 1, old.Y),
                Dir.RIGHT => new XY(old.X + 1, old.Y),
                Dir.DOWN => new XY(old.X, old.Y + 1), // fall
                Dir.UP => new XY(old.X, old.Y - 1), // unused
                _ => old
            };
            CurrentRock.Offset = offsetNew;
        }

        /*
         * Returns true if movement in given direction would result in collision with wall or floor.
         */
        private bool ChamberWallPreventsMove(Dir direction)
        {
            var rock = CurrentRock;
            if (rock == null) return false;

            return direction switch
            {
                Dir.LEFT => rock.Offset.X < 1,
                Dir.RIGHT => rock.Offset.X >= WIDTH - rock.Rock.GetWidth(),
                Dir.DOWN => rock.Offset.Y >= State.Count - rock.Rock.GetHeight(),
                _ => false
            };
        }

        private bool CurrentRockOverlaysTower()
        {
            return CurrentRock?.Rock.GetCoords().Any(
                coord => State.ElementAt(CurrentRock.Offset.Y + coord.Y).ElementAt(CurrentRock.Offset.X + coord.X)
            ) ?? false;
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            foreach (XY coord in CurrentRock?.Rock.GetCoords() ?? Enumerable.Empty<XY>()) 
            {
                State.ElementAt(CurrentRock.Offset.Y + coord.Y)[CurrentRock.Offset.X + coord.X] = true;
            }
            CurrentRock = null;
        }


        /*
         * After some rocks have fallen, there will probably form an unreachable part of chamber. Such part of chamber
         * is irrelevant to further calculation, as no rock can fall there. This method locates this part and removes
         * it from chamber State. (To save memory)
         */
        public void RemoveUnreachablePartOfChamber()
        {
            var dist = Util.GetDistances<bool>(
                matrix: State.ToList(),
                start: new XY(0, 0),
                advancingRule: (_, neighbor) => !neighbor // reachable must be false, contain void
            );
            var yOfLastReachableRow = dist.TakeWhile(row => row.Any(value => value != int.MaxValue)).Count() - 1;
            CutChamberBelow(yOfLastReachableRow);
        }

        /*
         * Removes chamber below y
         */
        private void CutChamberBelow(int y)
        {
            int toRemove = State.Count - 1 - y;
            CutChamberFloors += toRemove;
            for (var i = 0; i < toRemove; i++)
            {
                State.RemoveLast();
            }
        }

        public void PrintState()
        {
            var symbols = new { Tower = '#', Void = '.', Rock = '@' };
            var chars = State.Select(row => row.Select(cell => cell ? symbols.Tower : symbols.Void).ToList()).ToList();
            foreach (var coord in CurrentRock?.Rock.GetCoords() ?? Enumerable.Empty<XY>())
            {
                chars[CurrentRock.Offset.Y + coord.Y][CurrentRock.Offset.X + coord.X] = symbols.Rock;
            }

            foreach (var row in chars)
            {
                Console.WriteLine("|" + string.Join("", row) + "|");
            }

            Console.WriteLine("+" + string.Join("", Enumerable.Repeat("-", WIDTH)) + "+");
            Console.WriteLine($"tower height: {RockTowerHeight}");
        }
    }

    public class CurrentRock
    {
        public Rock Rock { get; set; }
        public XY Offset { get; set; }

        public CurrentRock(Rock rock, XY offset)
        {
            Rock = rock;
            Offset = offset;
        }
    }

    /*
      Rock      #### | .#. | ..# | # | ##
      shapes:        | ### | ..# | # | ##
                     | .#. | ### | # | 
                     |     |     | # | 
    */
    public enum Rock { MINUS, PLUS, L   , I , SQUARE }

    static class RockExtensions
    {
        /*
         * Returns set of 2D coordinates representing each pixel of the shape offset from top left corner
         * Each shape has origin (0,0) in top-left corner
         */
        public static HashSet<XY> GetCoords(this Rock rock)
        {
            switch (rock)
            {
                case Rock.MINUS: return new HashSet<XY> { new(0, 0), new(1, 0), new(2, 0), new(3, 0) };
                case Rock.PLUS: return new HashSet<XY> { new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2) };
                case Rock.L: return new HashSet<XY> { new(2, 0), new(2, 1), new(2, 2), new(1, 2), new(0, 2) };
                case Rock.I: return new HashSet<XY> { new(0, 0), new(0, 1), new(0, 2), new(0, 3) };
                case Rock.SQUARE: return new HashSet<XY> { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };
                default: return new HashSet<XY>();
            }
        }

        /*
         * Hard coded because the shapes will hardly ever change
         */
        public static int GetHeight(this Rock rock)
        {
            switch (rock)
            {
                case Rock.MINUS: return 1;
                case Rock.PLUS: return 3;
                case Rock.L: return 3;
                case Rock.I: return 4;
                case Rock.SQUARE: return 2;
                default: return 0;
            }
        }

        /*
         * Hard coded because the shapes will hardly ever change
         */
        public static int GetWidth(this Rock rock)
        {
            switch (rock)
            {
                case Rock.MINUS: return 4;
                case Rock.PLUS: return 3;
                case Rock.L: return 3;
                case Rock.I: return 1;
                case Rock.SQUARE: return 2;
                default: return 0;
            }
        }

        public static List<Rock> GetOrderedList()
        {
            return new List<Rock>() { Rock.MINUS, Rock.PLUS, Rock.L, Rock.I, Rock.SQUARE };
        }
    }
}
