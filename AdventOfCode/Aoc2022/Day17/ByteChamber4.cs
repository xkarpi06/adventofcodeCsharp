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
     * spot in chamber (0 => empty space, 1 => tower). The chamber is 7 wide so only 7 LSB (right bits) are being used
     *
     * Unlike ByteChamber3, the rock is stored as int and the chamber has not only _state, but also _context,
     * which is area from the _state around the current rock stored as int.
     *
     * This class is also hard-coded to be 7-WIDE and would be complicated to change the WIDTH.
     * Max supported rock height is 4, as the rock is stored in int with 1 byte per "floor".
     *
     * Storing rock as int enables
     * - quick bitwise comparison with _context
     * - quick moving left & right using bitwise shift
     * Storing _context enables
     * - quick update when the rock falls 1 down by shifting values << 8 and adding the new byte as the LSB of int
     */
    public class ByteChamber4 : IChamber
    {
        private const int OFFSET_BOTTOM = 3;
        // when a rock has positive bitwise 'and' with this mask, it cannot move left
        private const int LEFT_WALL_MASK = 1_077_952_576; // 01000000 01000000 01000000 01000000
        // when a rock has positive bitwise 'and' with this mask, it cannot move right
        private const int RIGHT_WALL_MASK = 16_843_009;   // 00000001 00000001 00000001 00000001
        private CurrentRock4 _currentRock = new();
        private long _cutChamberFloors;
        private List<byte> _state = new();
        private int _topVoidRows
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
                return topVoidRows;
            }
        }

        public List<List<bool>> State =>
            _state.Select(
                row => Enumerable.Range(0, 7).Select(i => ((row >> i) & 1) == 1).Reverse().ToList()
            ).Reverse().ToList();

        public long RockTowerHeight => _state.Count - _topVoidRows + _cutChamberFloors;

        /*
         * Each rock appears so that its left edge is two units (2) away from the left wall and its bottom
         * edge is three units (3) above the highest rock in the room (or the floor, if there isn't one).
         */
        public void SpawnRock(Rock rock)
        {
            var rockHeight = rock.GetHeight();
            var voidRowsNeeded = OFFSET_BOTTOM + rockHeight;
            var rowsMissing = voidRowsNeeded - _topVoidRows;

            for (var i = 0; i < rowsMissing; i++)
            {
                _state.Add(0); // int 0 means 0b00000000
            }
            // current rock update
            _currentRock.Height = rockHeight;
            _currentRock.Value = rock.GetIntValueFixedWidth7Offset2();
            // there may be up to 6 topVoidRows after previous rock. If this rock is flat ####, it is then placed to Y=2
            _currentRock.YOffset = rowsMissing > 0 ? 0 : 0 - rowsMissing;
            // Since the rock si spawned in "void", the _currentRock.ChamberContext is always 0 after spawn.
            _currentRock.ChamberContext = 0;
            _currentRock.Valid = true;
        }

        /*
         * Move rock if it can be moved (tower interference & walls prevent movement)
         * params checkWall and checkTower are ignored, because the check itself is just bitwise AND
         *
         * returns false if rock can't fall anymore
         */
        public bool MoveRockWithRules(Dir direction, bool checkWall, bool checkTower)
        {
            if (!_currentRock.Valid) return false;
            switch (direction)
            {
                case Dir.LEFT:
                case Dir.RIGHT:
                    bool moveLeft = direction == Dir.LEFT;
                    int wallMask = moveLeft ? LEFT_WALL_MASK : RIGHT_WALL_MASK;
                    // check if next to a wall, which could prevent move, leave if yes
                    if ((_currentRock.Value & wallMask) > 0) return true;
                    // move left/right
                    _currentRock.Value = moveLeft ? _currentRock.Value << 1 : _currentRock.Value >> 1;
                    // check tower collision, leave if there's no collision
                    if ((_currentRock.Value & _currentRock.ChamberContext) == 0) return true;
                    // move back (there was tower collision)
                    _currentRock.Value = moveLeft ? _currentRock.Value >> 1 : _currentRock.Value << 1;
                    return true;
                case Dir.DOWN:
                    // check if at the bottom of chamber, leave if yes
                    if (_currentRock.YOffset >= _state.Count - _currentRock.Height) return false;
                    // check if rock collides with tower if it moves down
                    var newByteIdx = _currentRock.YOffset + _currentRock.Height + 1; // get byte below rock
                    int newChamberContext = (_currentRock.ChamberContext << 8) | _state[^newByteIdx]; // add it to context
                    if ((_currentRock.Value & newChamberContext) == 0)
                    {
                        // no collision, move rock
                        _currentRock.YOffset++;
                        _currentRock.ChamberContext = newChamberContext;
                        return true;
                    }
                    return false;
                default: return true;
            }
        }

        /*
         * Mark current falling rock as stationary (which makes it a part of tower of rocks)
         */
        public void PutRockToSleep()
        {
            if (!_currentRock.Valid) return;
            for (int i = 0; i < _currentRock.Height; i++)
            {
                /*
                 *                                              rock:       chamber:
                 *                                              aaaaaaaa    00000000
                 *                                              bbbbXbbb    00000001
                 *                                              cccXXXcc    00000001
                 * rock: aaaaaaaa bbbbXbbb cccXXXcc ddddXddd -> ddddXddd    00000111
                 *
                 * each byte is combined with current state
                 * keep in mind the lowest part of rock is the highest index in _state, so iterate _state in reverse
                 * don't forget, the _state list itself is in reverse order (chamber top is the last element)
                 */
                var state_idx = _currentRock.YOffset + _currentRock.Height - i;
                // this disassembles the int byte after byte
                var rockByte = (byte)(_currentRock.Value >> (i * 8));
                _state[^state_idx] = (byte)(rockByte | _state[^state_idx]);
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
