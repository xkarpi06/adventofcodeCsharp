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
     *
     * In addition the current rock is at all times a Y-offset and a byte array such as:
     * 0b00001000
     * 0b00011100
     * 0b00001000
     */
    public class ByteChamber2 : IChamber
    {
        private const int OFFSET_LEFT = 2;
        private const int OFFSET_BOTTOM = 3;
        private const int WIDTH = 7;
        // when a rock is at this position, it cannot move left
        private const byte TOUCHING_LEFT_WALL = 1 << (WIDTH - 1); // 0b01000000
        // when a rock is at this position, it cannot move right
        private const byte TOUCHING_RIGHT_WALL = 1; // 0b00000001
        private CurrentRock2? _currentRock;
        private long _cutChamberFloors;
        private List<byte> _state;

        public List<List<bool>> State =>
            _state.Select(
                row => Enumerable.Range(0, WIDTH).Select(i => ((row >> i) & 1) == 1).Reverse().ToList()
            ).ToList();

        public long RockTowerHeight =>
            _state.Count - _state.TakeWhile(row => row == 0).Count() + _cutChamberFloors;

        public ByteChamber2()
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
            int topVoidRows = 0;
            foreach (var row in _state)
            {
                if (row > 0) break;
                topVoidRows++;
            }
            var voidRowsNeeded = OFFSET_BOTTOM + rock.GetHeight();
            var rowsMissing = voidRowsNeeded - topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                _state.Insert(0, 0); // int 0 means 0b00000000
            }
            // current rock init
            byte[] rockBytes = rock.GetCoordsByteArray();
            for (int i = 0; i < rockBytes.Length; i++)
            {
                rockBytes[i] >>= (8 - WIDTH + OFFSET_LEFT);
            }
            // there may be up to 6 topVoidRows after previous rock. If this rock is flat ####, it is then placed to Y=2
            _currentRock = new CurrentRock2(rockBytes, rowsMissing > 0 ? 0 : 0 - rowsMissing);
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
                case Dir.LEFT:
                case Dir.RIGHT:
                    {
                        for (int i = 0; i < _currentRock.bytes.Length; i++)
                        {
                            if (direction == Dir.LEFT)
                                _currentRock.bytes[i] <<= 1;
                            else
                                _currentRock.bytes[i] >>= 1;
                        }
                    }
                    break;
                case Dir.DOWN: _currentRock.YOffset++; break; // fall
                case Dir.UP: _currentRock.YOffset--; break;
                default: break;
            }
        }

        /*
         * Returns true if movement in given direction would result in collision with wall or floor.
         */
        private bool ChamberWallPreventsMove(Dir direction)
        {
            var rock = _currentRock;
            if (rock == null) return false;

            switch (direction)
            {
                case Dir.LEFT:
                case Dir.RIGHT:
                {
                    // check if any rock part is touching either left or right wall
                    byte comparator = (direction == Dir.LEFT) ? TOUCHING_LEFT_WALL : TOUCHING_RIGHT_WALL;
                    foreach (byte rockByte in rock.bytes)
                    {
                        if ((rockByte & comparator) > 0) return true;
                    }
                }
                    break;
                case Dir.DOWN:
                    return rock.YOffset >= _state.Count - rock.bytes.Length; // bytes.Length serves as rock height
                default: break;
            }
            return false;
        }

        private bool CurrentRockOverlaysTower()
        {
            if (_currentRock == null) return false;
            byte[] rockBytes = _currentRock.bytes;
            byte[] chamberSlice = _state.GetRange(_currentRock.YOffset, rockBytes.Length).ToArray();
            for (int i = 0; i < rockBytes.Length; i++)
            {
                if ((rockBytes[i] & chamberSlice[i]) > 0) return true;
            }
            return false;
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            if (_currentRock == null) return;
            byte[] rockBytes = _currentRock.bytes;
            byte[] chamberSlices = _state.GetRange(_currentRock.YOffset, rockBytes.Length).ToArray();
            for (int i = 0; i < rockBytes.Length; i++)
            {
                _state[_currentRock.YOffset + i] = (byte)(rockBytes[i] | chamberSlices[i]);
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
