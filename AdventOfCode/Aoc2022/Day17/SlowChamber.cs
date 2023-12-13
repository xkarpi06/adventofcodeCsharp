using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Aoc2022.Day17
{
    public class SlowChamber : IChamber
    {
        private const int OFFSET_LEFT = 2;
        private const int OFFSET_BOTTOM = 3;
        private const int WIDTH = 7;
        private CurrentRock? _currentRock;
        private long _cutChamberFloors;
        private LinkedList<List<bool>> _state;

        public List<List<bool>> State => _state.ToList();

        public long RockTowerHeight =>
            _state.Count - _state.TakeWhile(row => row.All(cell => !cell)).Count() + _cutChamberFloors;

        public SlowChamber()
        {
            _state = new LinkedList<List<bool>>();
            _currentRock = null;
            _cutChamberFloors = 0;
        }

        /*
         * Each rock appears so that its left edge is two units away from the left wall and its bottom
         * edge is three units above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock)
        {
            var topVoidRows = _state.TakeWhile(row => row.All(cell => !cell)).Count();
            var voidRowsNeeded = OFFSET_BOTTOM + rock.GetHeight();
            var rowsMissing = voidRowsNeeded - topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                _state.AddFirst(Enumerable.Repeat(false, WIDTH).ToList());
            }

            _currentRock = new CurrentRock(rock, new XY(OFFSET_LEFT, rowsMissing > 0 ? 0 : 0 - rowsMissing));
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
            if (_currentRock == null || ChamberWallPreventsMove(direction)) return;
            XY old = _currentRock.Offset;
            var offsetNew = direction switch
            {
                Dir.LEFT => new XY(old.X - 1, old.Y),
                Dir.RIGHT => new XY(old.X + 1, old.Y),
                Dir.DOWN => new XY(old.X, old.Y + 1), // fall
                Dir.UP => new XY(old.X, old.Y - 1), // unused
                _ => old
            };
            _currentRock.Offset = offsetNew;
        }

        /*
         * Returns true if movement in given direction would result in collision with wall or floor.
         */
        private bool ChamberWallPreventsMove(Dir direction)
        {
            var rock = _currentRock;
            if (rock == null) return false;

            return direction switch
            {
                Dir.LEFT => rock.Offset.X < 1,
                Dir.RIGHT => rock.Offset.X >= WIDTH - rock.Rock.GetWidth(),
                Dir.DOWN => rock.Offset.Y >= _state.Count - rock.Rock.GetHeight(),
                _ => false
            };
        }

        private bool CurrentRockOverlaysTower()
        {
            return _currentRock?.Rock.GetCoords().Any(
                coord => _state.ElementAt(_currentRock.Offset.Y + coord.Y).ElementAt(_currentRock.Offset.X + coord.X)
            ) ?? false;
            // var rockOffset = _currentRock.Offset;
            // var rockCoords = _currentRock?.Rock.GetCoords();
            // if (rockCoords == null) return false;
            // var i = 0;
            // var j = 0;
            // foreach (var row in _state)
            // {
            //     j = 0;
            //     foreach (var cell in row)
            //     {
            //         // if a part of rock is within current cell (i,j) and that cell contains tower (cell==true)
            //         // the rock overlays tower => return true;
            //         if (cell)
            //         {
            //             foreach (var coord in rockCoords)
            //             {
            //                 if (rockOffset.Y + coord.Y == i && rockOffset.X + coord.X == j) return true;
            //             }
            //         }
            //         j++;
            //     }
            //     i++;
            // }
            // return false;
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            foreach (XY coord in _currentRock?.Rock.GetCoords() ?? Enumerable.Empty<XY>())
            {
                _state.ElementAt(_currentRock.Offset.Y + coord.Y)[_currentRock.Offset.X + coord.X] = true;
            }
            _currentRock = null;
        }


        /*
         * After some rocks have fallen, there will probably form an unreachable part of chamber. Such part of chamber
         * is irrelevant to further calculation, as no rock can fall there. This method locates this part and removes
         * it from chamber _state. (To save memory)
         */
        public void RemoveUnreachablePartOfChamber()
        {
            var dist = Util.GetDistances<bool>(
                matrix: _state.ToList(),
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
            int toRemove = _state.Count - 1 - y;
            _cutChamberFloors += toRemove;
            for (var i = 0; i < toRemove; i++)
            {
                _state.RemoveLast();
            }
        }
    }
}
