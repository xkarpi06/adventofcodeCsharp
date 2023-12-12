using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode.Aoc2022.Day17;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Helpers;

namespace AdventOfCode.Aoc2022.Day17.Tests
{

    /*
     * DEFAULT ASSUMPTION FOR EVERY TEST:
     *
     * Each rock appears so that its left edge is TWO (2) units away from the left wall and its bottom
     * edge is THREE (3) units above the highest rock in the room (or the floor, if there isn't one).
     */
    [TestClass()]
    public class TallNarrowChamberTests
    {
        private TallNarrowChamber Chamber;

        [TestInitialize]
        public void TestInitialize()
        {
            Chamber = new TallNarrowChamber();
        }

        // [TestCleanup]
        // public void TestCleanup()
        // {
        // }

        /*
         * Spawn rock, move it down 3 times and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void SpawnRockTest()
        {
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.PutRockToSleep();
            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,1,0,0,0},
                new() {0,0,1,1,1,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
        }

        /*
         * Spawn rock, let it fall below ground level (4 times) and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_FallBelowGround()
        {
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);    // this move is already into ground, should not be executed
            Chamber.PutRockToSleep();
            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,1,1,1,0,0},
                new() {0,0,0,1,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(3L, Chamber.RockTowerHeight);
        }

        /*
         * Spawn rock, let it collide with (fall onto) existing tower and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_FallOntoExistingTower()
        {
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            Chamber.SpawnRock(Rock.SQUARE);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN); // this move will collide with existing rock tower
            Chamber.PutRockToSleep();

            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,1,1,0,0,0},
                new() {0,0,1,1,0,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,1,1,1,0,0},
                new() {0,0,0,1,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(5L, Chamber.RockTowerHeight);
        }

        /*
         * Spawn rock, move it left beyond chamber wall and validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveBeyondLeftChamberWall()
        {
            Chamber.SpawnRock(Rock.SQUARE);
            Chamber.MoveRockWithRules(Dir.LEFT);
            Chamber.MoveRockWithRules(Dir.LEFT);
            Chamber.MoveRockWithRules(Dir.LEFT);    // this move will collide with left chamber wall
            Chamber.PutRockToSleep();

            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {1,1,0,0,0,0,0},
                new() {1,1,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
            };
            
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
        }

        /*
         * Spawn rock, move it right beyond chamber wall and validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveBeyondRightChamberWall()
        {
            Chamber.SpawnRock(Rock.MINUS);
            Chamber.MoveRockWithRules(Dir.RIGHT);
            Chamber.MoveRockWithRules(Dir.RIGHT);    // this move will collide with right chamber wall
            Chamber.PutRockToSleep();

            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,1,1,1,1},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
            };

            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
        }

        /*
         * Move right command to a falling rock will result in collision with existing tower. Freeze the rock,
         * then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveRightToCollideWithTower()
        {
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.MoveRockWithRules(Dir.RIGHT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.RIGHT); // this move will collide with existing rock tower
            Chamber.PutRockToSleep();

            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,1,1,1,0,0},
                new() {0,0,0,1,1,0,0},
                new() {0,0,0,1,1,1,0},
                new() {0,0,0,0,1,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(5L, Chamber.RockTowerHeight);
        }

        /*
         * Move left command to a falling rock will result in collision with existing tower. Freeze the rock,
         * then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveLeftToCollideWithTower()
        {
            Chamber.SpawnRock(Rock.PLUS);
            Chamber.MoveRockWithRules(Dir.LEFT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            Chamber.SpawnRock(Rock.SQUARE);
            Chamber.MoveRockWithRules(Dir.RIGHT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.LEFT); // this move will collide with existing rock tower
            Chamber.PutRockToSleep();

            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,1,1,0,0},
                new() {0,0,1,1,1,0,0},
                new() {0,1,1,1,0,0,0},
                new() {0,0,1,0,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(4L, Chamber.RockTowerHeight);
        }

        [TestMethod()]
        public void PutRockToSleepTest()
        {
            Chamber.SpawnRock(Rock.MINUS);
            Chamber.PutRockToSleep();
            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,1,1,1,1,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
        }

        /*
         * Create unreachable part of a cave, then try to remove it. Validate chamber state.
         */
        [TestMethod()]
        public void RemoveUnreachablePartOfChamberTest()
        {
            Chamber.SpawnRock(Rock.I);
            Chamber.MoveRockWithRules(Dir.RIGHT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            Chamber.SpawnRock(Rock.MINUS);
            Chamber.MoveRockWithRules(Dir.RIGHT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            Chamber.SpawnRock(Rock.MINUS);
            Chamber.MoveRockWithRules(Dir.LEFT);
            Chamber.MoveRockWithRules(Dir.LEFT);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.MoveRockWithRules(Dir.DOWN);
            Chamber.PutRockToSleep();
            List<List<int>> ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {1,1,1,1,0,0,0},
                new() {0,0,0,1,1,1,1},
                new() {0,0,0,1,0,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,0,1,0,0,0},
                new() {0,0,0,1,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(6L, Chamber.RockTowerHeight);
            Chamber.RemoveUnreachablePartOfChamber();
            ExpectedState = new()
            {
                // 0 - empty space, 1 - dead rock
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {0,0,0,0,0,0,0},
                new() {1,1,1,1,0,0,0},
            };
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(ExpectedState), Chamber.State.ToList()));
            Assert.AreEqual(6L, Chamber.RockTowerHeight);
        }

        // PrintPretty(Chamber.State.ToList(), "\t");
        // PrintPretty(ToBoolMatrix(ExpectedState), "\t");

        /*
         * Util function for printing 2D matrices.
         */
        private static void PrintPretty<T>(List<List<T>> list, string delimiter = "")
        {
            foreach (List<T> row in list)
            {
                Console.WriteLine(string.Join(delimiter, row));
            }
        }

        private List<List<bool>> ToBoolMatrix(List<List<int>> input) =>
            input.Select(row => row.Select(col => col == 1).ToList()).ToList();

        private bool ChamberEquality(List<List<bool>> expected, List<List<bool>> actual) =>
            expected.SequenceEqual(actual, new ListEqualityComparer<bool>());

        private class ListEqualityComparer<T> : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y) => x.SequenceEqual(y);

            public int GetHashCode(List<T> obj) => obj.Aggregate(0, (acc, val) => HashCode.Combine(acc, val.GetHashCode()));
        }
    }
}