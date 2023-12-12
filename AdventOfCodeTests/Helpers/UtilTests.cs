using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Helpers.Tests
{
    [TestClass]
    public class UtilTests
    {
        private List<List<int>> MatrixEmpty { get; init; } = new List<List<int>>
        {
            // 0 - empty space, 1 - rock tower
            new() {0,0,0,0,0},
            new() {0,0,0,0,0},
            new() {0,0,0,0,0},
            new() {0,0,0,0,0},
            new() {0,0,0,0,0}
        };

        private List<List<int>> MatrixEmptyDistances { get; init; } = new List<List<int>>
        {
            new() {0,1,2,3,4},
            new() {1,2,3,4,5},
            new() {2,3,4,5,6},
            new() {3,4,5,6,7},
            new() {4,5,6,7,8}
        };

        private List<List<int>> MatrixWithSimpleTower { get; init; } = new List<List<int>>
        {
            // 0 - empty space, 1 - rock tower
            new() {0,0,0,0,0},
            new() {0,1,1,1,0},
            new() {0,1,0,0,0},
            new() {0,1,1,0,0},
            new() {0,1,1,1,1}
        };
        
        private List<List<int>> MatrixWithSimpleTowerDistances { get; init; } = new List<List<int>>
        {
            new() {0,1,2,3,4},
            new() {1,9,9,9,5},
            new() {2,9,8,7,6},
            new() {3,9,9,8,7},
            new() {4,9,9,9,9}
        }   // replace every 9 with int.MaxValue
            .Select(row => row.Select(val => (val == 9) ? int.MaxValue : val).ToList()).ToList();

        private List<List<int>> MatrixWithUnreachableSpots { get; init; } = new List<List<int>>
        {
            // 0 - empty space, 1 - rock tower
            new() {0,0,0,0,0},
            new() {0,1,1,1,0},
            new() {0,1,0,1,1},  // unreachable pocket at (2,2)
            new() {0,0,1,1,0},  // unreachable pocket at (3,4)
            new() {0,1,1,1,1}
        };

        private List<List<int>> MatrixWithUnreachableSpotsDistances { get; init; } = new List<List<int>>
        {
            new() {0,1,2,3,4},
            new() {1,9,9,9,5},
            new() {2,9,9,9,9},
            new() {3,4,9,9,9},
            new() {4,9,9,9,9}
        }   // replace every 9 with int.MaxValue
            .Select(row => row.Select(val => (val == 9) ? int.MaxValue : val).ToList()).ToList();

        private Func<bool, bool, bool> DefaultAdvancingRule { get; } = (_, neighbor) => !neighbor;
        private XY DefaultStart { get; init; } = new XY(0, 0);

        [TestMethod]
        public void GetDistancesTest_MatrixEmpty()
        {
            List<List<int>> distances = Util.GetDistances<bool>(
                matrix: ToBoolMatrix(MatrixEmpty),
                start: DefaultStart,
                advancingRule: DefaultAdvancingRule
            );
            Assert.IsTrue(IntListEquality(MatrixEmptyDistances, distances));
        }

        /*
         * GetDistances algorithm should discover all spots around simple tower and return correct distance to every spot
         */
        [TestMethod]
        public void GetDistancesTest_MatrixWithSimpleTower()
        {
            List<List<int>> distances = Util.GetDistances<bool>(
                matrix: ToBoolMatrix(MatrixWithSimpleTower),
                start: DefaultStart,
                advancingRule: DefaultAdvancingRule
            );
            Assert.IsTrue(IntListEquality(MatrixWithSimpleTowerDistances, distances));
        }

        /*
         * Rock tower creates unreachable pockets and GetDistances algorithm should mark all unreachable spots with
         * int.MaxValue
         */
        [TestMethod]
        public void GetDistancesTest_MatrixWithUnreachableSpots()
        {
            List<List<int>> distances = Util.GetDistances<bool>(
                matrix: ToBoolMatrix(MatrixWithUnreachableSpots),
                start: DefaultStart,
                advancingRule: DefaultAdvancingRule
            );
            Assert.IsTrue(IntListEquality(MatrixWithUnreachableSpotsDistances, distances));
        }

        private List<List<bool>> ToBoolMatrix(List<List<int>> input) =>
            input.Select(row => row.Select(col => col == 1).ToList()).ToList();

        private bool IntListEquality(List<List<int>> expected, List<List<int>> actual) =>
            expected.SequenceEqual(actual, new ListEqualityComparer<int>());

        private class ListEqualityComparer<T> : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y) => x.SequenceEqual(y);

            public int GetHashCode(List<T> obj) => obj.Aggregate(0, (acc, val) => HashCode.Combine(acc, val.GetHashCode()));
        }
    }
}