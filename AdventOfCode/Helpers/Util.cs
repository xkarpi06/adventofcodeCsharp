using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Helpers
{
    public static class Util
    {
        /*
         * Computes Manhattan distance from start to each reachable point in matrix and returns matrix of these distances.
         * For unreachable points in the matrix, their distance from start will be Int.MaxValue.
         *
         * Algorithm: BFS from the start point
         * for current point finds new reachable points by [advancingRule].
         *
         * @param start starting position in matrix to count distance from
         * @param advancingRule rule for advancing from one point to its neighbor. If true, distance from the current
         * to neighbor is 1. bool advancingRule(T current, T neighbor)
         */
        public static List<List<int>> GetDistances<T>(
            List<List<T>> matrix,
            XY start,
            Func<T, T, bool> advancingRule)
        {
            List<List<int>> dist = new List<List<int>>(matrix.Count);
            for (int i = 0; i < matrix.Count; i++)
            {
                dist.Add(new List<int>(matrix[0].Count));
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    dist[i].Add(int.MaxValue);
                }
            }

            dist[start.Y][start.X] = 0;
            HashSet<XY> visited = new HashSet<XY>();
            Queue<XY> queue = new Queue<XY>(new[] { start });

            while (queue.Count > 0)
            {
                XY current = queue.Dequeue();

                // Find neighbors
                List<XY> reachable = new List<XY>
                {
                    new XY(current.X - 1, current.Y),
                    new XY(current.X + 1, current.Y),
                    new XY(current.X, current.Y - 1),
                    new XY(current.X, current.Y + 1)
                }.FindAll(it =>
                    it.X >= 0 && it.X < dist[0].Count &&
                    it.Y >= 0 && it.Y < dist.Count &&
                    !queue.Contains(it) &&
                    !visited.Contains(it) &&
                    advancingRule(matrix[current.Y][current.X], matrix[it.Y][it.X]));

                // Set distance to neighbor & add to queue
                foreach (XY neighbor in reachable)
                {
                    dist[neighbor.Y][neighbor.X] = dist[current.Y][current.X] + 1;
                    queue.Enqueue(neighbor);
                }

                visited.Add(current);
            }

            return dist;
        }

        /*
         * Prints 2D matrix of integers separated by tab & for values Int.MaxValue prints 'max'.
         */
        public static void PrintDistances(List<List<int>> dist)
        {
            PrintPretty(dist.ConvertAll(row => row.ConvertAll(cell => cell == int.MaxValue ? "max" : cell.ToString())), "\t");
        }

        /*
         * For a 2D matrix of integers, prints 'X' for values Int.MaxValue and '.' otherwise. Tucked without spaces.
         */
        public static void PrintDiscovered(List<List<int>> dist)
        {
            PrintPretty(dist.ConvertAll(row => row.ConvertAll(cell => cell == int.MaxValue ? "X" : ".")));
        }

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
    }
}