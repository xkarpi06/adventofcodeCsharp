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
    public interface IChamber
    {
        public List<List<bool>> State { get; }
        public long RockTowerHeight { get; }

        /*
         * Each rock appears so that its left edge is two units away from the left wall and its bottom
         * edge is three units above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock);

        /*
         * Move rock if it can be moved (tower interference & walls prevent movement)
         * returns false if rock can't fall anymore
         */
        public bool MoveRockWithRules(Dir direction);

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep();

        /*
         * After some rocks have fallen, there will probably form an unreachable part of chamber. Such part of chamber
         * is irrelevant to further calculation, as no rock can fall there. This method locates this part and removes
         * it from chamber State. (To save memory)
         */
        public void RemoveUnreachablePartOfChamber();
    }
}
