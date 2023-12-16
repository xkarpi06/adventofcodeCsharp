using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode.Aoc2022.Day17;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Helpers;

namespace AdventOfCodeTests.Aoc2022
{

    /*
     * DEFAULT ASSUMPTION FOR EVERY TEST:
     *
     * Each rock appears so that its left edge is TWO (2) units away from the left wall and its bottom
     * edge is THREE (3) units above the highest rock in the room (or the floor, if there isn't one).
     */
    [TestClass()]
    public class ChamberTests
    {
        private IChamber _chamber;

        [TestInitialize]
        public void TestInitialize()
        {
            _chamber = new ByteChamber3();
            // _chamber = new ByteChamber2();
            // _chamber = new ByteChamber();
            // _chamber = new SlowChamber();
        }

        // [TestCleanup]
        // public void TestCleanup()
        // {
        // }

        /*
         * Spawn rock and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void SpawnRockTest()
        {
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.PutRockToSleep();
            const string expectedState = """
                                         ...X...
                                         ..XXX..
                                         ...X...
                                         .......
                                         .......
                                         .......
                                         """;
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
        }

        /*
         * Spawn rock and move it down to a free space, the MoveRockWithRules method should return correct value.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_ReturnTrue()
        {
            _chamber.SpawnRock(Rock.PLUS);
            bool x = _chamber.MoveRockWithRules(Dir.DOWN);
            Assert.IsTrue(x);
        }

        /*
         * Spawn rock, let it fall below ground level (4 times) and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_FallBelowGround()
        {
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            bool x = _chamber.MoveRockWithRules(Dir.DOWN);    // this move is already into ground, should not be executed
            Assert.IsFalse(x);
            _chamber.PutRockToSleep();
            const string expectedState = """
                                         .......
                                         .......
                                         .......
                                         ...X...
                                         ..XXX..
                                         ...X...
                                         """;
            PrintPrettyBool(_chamber.State);
            PrintPrettyString(expectedState);
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(3L, _chamber.RockTowerHeight);
        }

        /*
         * Spawn rock, let it collide with (fall onto) existing tower and freeze it. Then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_FallOntoExistingTower()
        {
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.SQUARE);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            bool x = _chamber.MoveRockWithRules(Dir.DOWN); // this move will collide with existing rock tower
            Assert.IsFalse(x);
            _chamber.PutRockToSleep();
            const string expectedState = """
                                         .......
                                         .......
                                         .......
                                         ..BB...
                                         ..BB...
                                         ...A...
                                         ..AAA..
                                         ...A...
                                         """;
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(5L, _chamber.RockTowerHeight);
        }

        /*
         * Spawn rock A that will be higher than rock B. Put rock A to sleep next to a wall. Rock B will spawn
         * away from that wall and should not have a problem to move towards it. (This was a bu.g in ByteChamber3)
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_PreviousRockDoesNotEffectCurrent()
        {
            _chamber.SpawnRock(Rock.SQUARE);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.MoveRockWithRules(Dir.LEFT); 
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.MINUS);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.PutRockToSleep();
            const string expectedState = """
                                         .BBBB..
                                         .......
                                         .......
                                         .......
                                         AA.....
                                         AA.....
                                         """;
            PrintPrettyBool(_chamber.State);
            PrintPrettyString(expectedState);
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
        }

        /*
         * Spawn rock, move it left beyond chamber wall and validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveBeyondLeftChamberWall()
        {
            _chamber.SpawnRock(Rock.SQUARE);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.MoveRockWithRules(Dir.LEFT);
            bool x = _chamber.MoveRockWithRules(Dir.LEFT);    // this move will collide with left chamber wall
            Assert.IsTrue(x);   // side collision does not prevent down movement
            _chamber.PutRockToSleep();

            const string expectedState = """
                                         XX.....
                                         XX.....
                                         .......
                                         .......
                                         .......
                                         """;
            
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
        }

        /*
         * Spawn rock, move it right beyond chamber wall and validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveBeyondRightChamberWall()
        {
            _chamber.SpawnRock(Rock.MINUS);
            _chamber.MoveRockWithRules(Dir.RIGHT);
            bool x = _chamber.MoveRockWithRules(Dir.RIGHT);    // this move will collide with right chamber wall
            Assert.IsTrue(x);   // side collision does not prevent down movement
            _chamber.PutRockToSleep();

            const string expectedState = """
                                         ...XXXX
                                         .......
                                         .......
                                         .......
                                         """;
            
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
        }

        /*
         * Move right command to a falling rock will result in collision with existing tower. Freeze the rock,
         * then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveRightToCollideWithTower()
        {
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.MoveRockWithRules(Dir.RIGHT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            bool x = _chamber.MoveRockWithRules(Dir.RIGHT); // this move will collide with existing rock tower
            Assert.IsTrue(x);   // side collision does not prevent down movement
            _chamber.PutRockToSleep();

            const string expectedState = """
                                         .......
                                         .......
                                         .......
                                         .......
                                         ...B...
                                         ..BBB..
                                         ...BA..
                                         ...AAA.
                                         ....A..
                                         """;
            
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(5L, _chamber.RockTowerHeight);
        }

        /*
         * Move left command to a falling rock will result in collision with existing tower. Freeze the rock,
         * then validate chamber state.
         */
        [TestMethod()]
        public void MoveRockWithRulesTest_MoveLeftToCollideWithTower()
        {
            _chamber.SpawnRock(Rock.PLUS);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.SQUARE);
            _chamber.MoveRockWithRules(Dir.RIGHT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            bool x = _chamber.MoveRockWithRules(Dir.LEFT); // this move will collide with existing rock tower
            Assert.IsTrue(x);   // side collision does not prevent down movement
            _chamber.PutRockToSleep();

            const string expectedState = """
                                         .......
                                         .......
                                         .......
                                         .......
                                         ...BB..
                                         ..ABB..
                                         .AAA...
                                         ..A....
                                         """;

            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(4L, _chamber.RockTowerHeight);
        }

        [TestMethod()]
        public void PutRockToSleepTest()
        {
            _chamber.SpawnRock(Rock.MINUS);
            _chamber.PutRockToSleep();

            const string expectedState = """
                                         ..XXXX.
                                         .......
                                         .......
                                         .......
                                         """;

            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
        }

        /*
         * Create unreachable part of a chamber, then try to remove it. Validate chamber state.
         */
        [TestMethod()]
        public void RemoveUnreachablePartOfChamberTest()
        {
            _chamber.SpawnRock(Rock.I);
            _chamber.MoveRockWithRules(Dir.RIGHT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.MINUS);
            _chamber.MoveRockWithRules(Dir.RIGHT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            _chamber.SpawnRock(Rock.MINUS);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.MoveRockWithRules(Dir.LEFT);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.MoveRockWithRules(Dir.DOWN);
            _chamber.PutRockToSleep();
            var expectedState = """
                                .......
                                .......
                                .......
                                CCCC...
                                ...BBBB
                                ...A...
                                ...A...
                                ...A...
                                ...A...
                                """;
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(6L, _chamber.RockTowerHeight);
            _chamber.RemoveUnreachablePartOfChamber();
            expectedState = """
                            .......
                            .......
                            .......
                            CCCC...
                            """;
            PrintPrettyBool(_chamber.State);
            PrintPrettyString(expectedState);
            Assert.IsTrue(ChamberEquality(ToBoolMatrix(expectedState), _chamber.State));
            Assert.AreEqual(6L, _chamber.RockTowerHeight);
        }

        private void PrintPrettyString(string input)
        {
            PrintPrettyBool(ToBoolMatrix(input));
        }

        private void PrintPrettyBool(List<List<bool>> list, string delimiter = "")
        {
            PrintPretty(list.Select(x => x.Select(y => y ? 'X' : '.').ToList()).ToList());
        }

        /*
         * Util function for printing 2D matrices.
         */
        private void PrintPretty<T>(List<List<T>> list, string delimiter = "")
        {
            foreach (List<T> row in list)
            {
                Console.WriteLine(string.Join(delimiter, row));
            }
        }

        private List<List<bool>> ToBoolMatrix(List<List<int>> input) =>
            input.Select(row => row.Select(col => col == 1).ToList()).ToList();

        /*
         * Anything other than '.' will be true. '.' is false.
         */
        private List<List<bool>> ToBoolMatrix(string input)
        {
            const char EMPTY = '.';
            return input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(
                line => line.Select(c => c != EMPTY).ToList()
            ).ToList();
        }

        private bool ChamberEquality(List<List<bool>> expected, List<List<bool>> actual) =>
            expected.SequenceEqual(actual, new ListEqualityComparer<bool>());

        private class ListEqualityComparer<T> : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y) => x.SequenceEqual(y);

            public int GetHashCode(List<T> obj) => obj.Aggregate(0, (acc, val) => HashCode.Combine(acc, val.GetHashCode()));
        }
    }
}
