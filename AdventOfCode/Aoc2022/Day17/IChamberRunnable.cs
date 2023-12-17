using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Aoc2022.Day17
{
    /**
     * There are falling rocks (one at a time) inside a tall chamber and a tower made of stationary fallen rocks.
     * Falling rock is pushed by hot jets to left and right while falling.
     */
    public interface IChamberRunnable
    {
        /*
         * The chamber state is a 2D matrix where
         *  true = piece of rock tower
         *  false = empty space
         */
        public List<List<bool>> State { get; }
        public long RockTowerHeight { get; }

        /*
         * Run the whole simulation at once without the ability to step each manually
         */
        public void RunSimulation(long rockAmount, string jetBursts);

        /*
         * Prints chamber state in user-friendly way
         */
        public void PrintState()
        {
            foreach (var row in State.Select(x => x.Select(y => y ? 'X' : '.').ToList()).ToList())
            {
                Console.WriteLine(string.Join("", row));
            }
        }
    }
}
