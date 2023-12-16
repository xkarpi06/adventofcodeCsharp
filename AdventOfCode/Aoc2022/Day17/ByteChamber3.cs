using AdventOfCode.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Aoc2022.Day17
{
    /*
     * Chamber is represented by a column of Bytes, each byte represents one row in chamber where 0s and 1s represent 
     * spot in chamber (0 => empty space, 1 => tower). The chamber is 7 wide so only 7 least significant (right) bits
     * are being used.
     *
     * This class improves ByteChamber2 by:
     * - reducing object allocations (at _currentRock)
     * - reverting the list representing _state (last element of list is the top of chamber now)
     */
    public class ByteChamber3 : IChamber
    {
        private const int OFFSET_LEFT = 2;
        private const int OFFSET_BOTTOM = 3;
        private const int WIDTH = 7;
        // when a rock is at this position, it cannot move left
        private const byte TOUCHING_LEFT_WALL = 1 << (WIDTH - 1); // 0b01000000
        // when a rock is at this position, it cannot move right
        private const byte TOUCHING_RIGHT_WALL = 1; // 0b00000001
        private CurrentRock3 _currentRock;
        private long _cutChamberFloors;
        private List<byte> _state;

        public List<List<bool>> State =>
            _state.Select(
                row => Enumerable.Range(0, WIDTH).Select(i => ((row >> i) & 1) == 1).Reverse().ToList()
            ).Reverse().ToList();

        public long RockTowerHeight
        {
            get
            {
                int topVoidRows = 0;
                for (int i = 0; i < _state.Count; i++)
                {
                    if (_state[^(i + 1)] > 0)
                    {
                        topVoidRows = i;
                        break;
                    }
                }
                return _state.Count - topVoidRows + _cutChamberFloors;
            }
        }

        public ByteChamber3()
        {
            _state = new List<byte>();
            _currentRock = new CurrentRock3(highestRock: 4); // the I rock is the highest rock
            _cutChamberFloors = 0;
        }

        /*
         * Each rock appears so that its left edge is two units (2) away from the left wall and its bottom
         * edge is three units (3) above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock)
        {
            int topVoidRows = 0;
            for (int i = 0; i < _state.Count; i++)
            {
                if (_state[^(i + 1)] > 0)
                {
                    topVoidRows = i;
                    break;
                }
            }

            var rockHeight = rock.GetHeight();
            var voidRowsNeeded = OFFSET_BOTTOM + rockHeight;
            var rowsMissing = voidRowsNeeded - topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                _state.Add(0); // int 0 means 0b00000000
            }
            // current rock init
            byte[] rockBytes = rock.GetCoordsByteArray3();
            _currentRock.Height = rockHeight;
            for (int i = 0; i < _currentRock.Height; i++)
            {
                _currentRock.Bytes[i] = rockBytes[i];
                _currentRock.Bytes[i] >>= 8 - WIDTH + OFFSET_LEFT;
            }
            // there may be up to 6 topVoidRows after previous rock. If this rock is flat ####, it is then placed to Y=2
            _currentRock.YOffset = rowsMissing > 0 ? 0 : 0 - rowsMissing;
            _currentRock.Valid = true;

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
            if (!_currentRock.Valid) return;
            switch (direction)
            {
                case Dir.LEFT:
                case Dir.RIGHT:
                    {
                        for (int i = 0; i < _currentRock.Height; i++)
                        {
                            if (direction == Dir.LEFT)
                                _currentRock.Bytes[i] <<= 1;
                            else
                                _currentRock.Bytes[i] >>= 1;
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
            if (!_currentRock.Valid) return false;

            switch (direction)
            {
                case Dir.LEFT:
                case Dir.RIGHT:
                {
                    // check if any rock part is touching either left or right wall
                    byte comparator = (direction == Dir.LEFT) ? TOUCHING_LEFT_WALL : TOUCHING_RIGHT_WALL;
                    for (int i = 0; i < _currentRock.Height; i++)
                    {
                        if ((_currentRock.Bytes[i] & comparator) > 0) return true;
                    }
                }
                    break;
                case Dir.DOWN:
                    return _currentRock.YOffset >= _state.Count - _currentRock.Height;
                default: break;
            }
            return false;
        }

        private bool CurrentRockOverlaysTower()
        {
            if (!_currentRock.Valid) return false;
            for (int i = 0; i < _currentRock.Height; i++)
            {
                if ((_currentRock.Bytes[i] & _state[^(_currentRock.YOffset + i + 1)]) > 0) return true;
            }
            return false;
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            if (!_currentRock.Valid) return;
            for (int i = 0; i < _currentRock.Height; i++)
            {
                var state_idx = _currentRock.YOffset + i + 1;
                _state[^state_idx] = (byte)(_currentRock.Bytes[i] | _state[^state_idx]);
            }
            _currentRock.Valid = false;
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
            _state.RemoveRange(0,  toRemove);
        }
    }
}
