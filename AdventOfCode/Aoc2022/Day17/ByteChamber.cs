using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Aoc2022.Day17
{
    /*
     * Chamber is represented by a column of bytes, each byte represents one row in chamber where 0s and 1s represent 
     * spot in chamber (0 => empty space, 1 => tower). The chamber is 7 wide so only 7 least significant (right) bits
     * are being used.
     */
    public class ByteChamber : IChamber
    {
        private const int OFFSET_LEFT = 2;
        private const int OFFSET_BOTTOM = 3;
        private const int WIDTH = 7;
        private CurrentRock? _currentRock;
        private long _cutChamberFloors;
        private List<byte> _state;

        public List<List<bool>> State =>
            _state.Select(
                row => Enumerable.Range(0, WIDTH).Select(i => ((row >> i) & 1) == 1).Reverse().ToList()
            ).ToList();

        public long RockTowerHeight =>
            _state.Count - _state.TakeWhile(row => row == 0).Count() + _cutChamberFloors;

        public ByteChamber()
        {
            _state = new List<byte>();
            _currentRock = null;
            _cutChamberFloors = 0;
        }

        /*
         * Each rock appears so that its left edge is two units (2) away from the left wall and its bottom
         * edge is three units (3) above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock)
        {
            var topVoidRows = _state.TakeWhile(row => row == 0).Count();
            var voidRowsNeeded = OFFSET_BOTTOM + rock.GetHeight();
            var rowsMissing = voidRowsNeeded - topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                _state.Insert(0, 0); // int 0 means 0b00000000
            }

            _currentRock = new CurrentRock(rock, new XY(OFFSET_LEFT, rowsMissing > 0 ? 0 : 0 - rowsMissing));
        }

        /*
         * Move rock if it can be moved (tower interference & walls prevent movement)
         * returns false if rock can't fall anymore
         */
        public bool MoveRockWithRules(Dir direction, bool checkWall, bool checkTower)
        {
            // check if rock can move (checks only walls and bottom)
            if (checkWall && ChamberWallPreventsMove(direction)) return direction != Dir.DOWN;
            // move rock
            MoveRock(direction);
            // check tower interference & undo move if needed
            if (!checkTower || !CurrentRockOverlaysTower()) return true;
            MoveRock(direction.Opposite());
            return direction != Dir.DOWN;
        }

        /*
         * moves rock without checking interference or walls
         */
        private void MoveRock(Dir direction)
        {
            if (_currentRock == null) return;
            switch (direction)
            {
                case Dir.LEFT: _currentRock.Offset.X--; break;
                case Dir.RIGHT: _currentRock.Offset.X++; break; 
                case Dir.DOWN: _currentRock.Offset.Y++; break; // fall
                case Dir.UP: _currentRock.Offset.Y--; break; // unused
                default: break;
            };
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
            if (_currentRock == null) return false;
            byte[] rockBytes = _currentRock.Rock.GetCoordsByteArray();
            for (int i = 0; i < rockBytes.Length; i++)
            {
                byte updatedRockByte = ApplyRockOffset(rockBytes[i]);
                // get corresponding row in chamber
                byte chamberRow = _state.ElementAt(_currentRock.Offset.Y + i);
                // compare rock & chamber state at the row
                if ((updatedRockByte & chamberRow) > 0) return true;
            }
            return false;
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            byte[] rockBytes = _currentRock.Rock.GetCoordsByteArray();
            for (int i = 0; i < rockBytes.Length; i++)
            {
                byte updatedRockByte = ApplyRockOffset(rockBytes[i]);
                byte chamberRow = _state.ElementAt(_currentRock.Offset.Y + i);
                _state[_currentRock.Offset.Y + i] = (byte) (chamberRow | updatedRockByte);
            }
            _currentRock = null;
        }

        /*
         * rockCoords are normalized to top left corner, (such as 0b11000000) so we need to account for chamber
         * width (7) and rock offset first => chamber is at 7 LSB (right bits)
         */
        private byte ApplyRockOffset(byte rockByte)
        {
            byte updatedRockByte = (byte)(rockByte >> (8 - WIDTH)); // place rock by chamber's left wall
            return (byte)(updatedRockByte >> _currentRock.Offset.X);
        }

        /*
         * After some rocks have fallen, there will probably form an unreachable part of chamber. Such part of chamber
         * is irrelevant to further calculation, as no rock can fall there. This method locates this part and removes
         * it from chamber _state. (To save memory)
         */
        public void RemoveUnreachablePartOfChamber()
        {
            var dist = Util.GetDistances<bool>(
                matrix: State,
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
            _state.RemoveRange(_state.Count - toRemove, toRemove);
        }
    }
}
