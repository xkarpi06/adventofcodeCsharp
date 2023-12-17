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
     * Duplicate of ByteChamber4, but reduces function calls to minimum
     */
    public class ByteChamber5 : IChamberRunnable
    {
        private const int OFFSET_BOTTOM = 3;
        // when a rock has positive bitwise 'and' with this mask, it cannot move left
        private const int LEFT_WALL_MASK = 1_077_952_576; // 01000000 01000000 01000000 01000000
        // when a rock has positive bitwise 'and' with this mask, it cannot move right
        private const int RIGHT_WALL_MASK = 16_843_009;   // 00000001 00000001 00000001 00000001
        private CurrentRock4 _currentRock = new();
        private long _cutChamberFloors = 0;
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
         * Run the whole simulation at once without unnecessary function calls
         */
        public void RunSimulation(long rockAmount, string jetBursts)
        {
            var orderedRocks = RockExtensions.GetOrderedList();
            int jetCount = 0;
            int rockIndex = 0;
            bool canMoveDown;

            for (long i = 0; i < rockAmount; i++)
            {
                // region SPAWN ROCK
                var rock = orderedRocks[rockIndex++ % orderedRocks.Count];
                var rockHeight = rock.GetHeight();
                var voidRowsNeeded = OFFSET_BOTTOM + rockHeight;
                var rowsMissing = voidRowsNeeded - _topVoidRows;

                for (var j = 0; j < rowsMissing; j++)
                    _state.Add(0);
                _currentRock.Height = rockHeight;
                _currentRock.Value = rock.GetIntValueFixedWidth7Offset2();
                _currentRock.YOffset = rowsMissing > 0 ? 0 : 0 - rowsMissing;
                _currentRock.ChamberContext = 0;
                _currentRock.Valid = true;
                // endregion
                do
                {
                    // region MOVE LEFT/RIGHT
                    bool moveLeft = jetBursts[jetCount++ % jetBursts.Length] == '<';
                    int mask = moveLeft ? LEFT_WALL_MASK : RIGHT_WALL_MASK;
                    if ((_currentRock.Value & mask) == 0)
                    {
                        _currentRock.Value = moveLeft ? _currentRock.Value << 1 : _currentRock.Value >> 1;
                        if ((_currentRock.Value & _currentRock.ChamberContext) > 0)
                            _currentRock.Value = moveLeft ? _currentRock.Value >> 1 : _currentRock.Value << 1;
                    }
                    // endregion
                    // region MOVE DOWN
                    if (_currentRock.YOffset < _state.Count - _currentRock.Height)
                    {
                        var newByteIdx = _currentRock.YOffset + _currentRock.Height + 1;
                        int newChamberContext = (_currentRock.ChamberContext << 8) | _state[^newByteIdx];
                        if ((_currentRock.Value & newChamberContext) == 0)
                        {
                            _currentRock.YOffset++;
                            _currentRock.ChamberContext = newChamberContext;
                            canMoveDown = true;
                        }
                        else canMoveDown = false;
                    }
                    else canMoveDown = false;
                    // endregion
                } while (canMoveDown);

                // region PUT ROCK TO SLEEP
                if (!_currentRock.Valid) return;
                for (int k = 0; k < _currentRock.Height; k++)
                {
                    var state_idx = _currentRock.YOffset + _currentRock.Height - k;
                    var rockByte = (byte)(_currentRock.Value >> (k * 8));
                    _state[^state_idx] = (byte)(rockByte | _state[^state_idx]);
                }
                _currentRock.Valid = false;
                // endregion
            }

            // Console.WriteLine($"Rocks: {rockAmount}, Height: {RockTowerHeight}");
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
