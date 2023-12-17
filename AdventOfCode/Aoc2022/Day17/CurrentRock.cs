using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Aoc2022.Day17
{
    public class CurrentRock
    {
        public Rock Rock { get; set; }
        public XY Offset { get; set; }

        public CurrentRock(Rock rock, XY offset)
        {
            Rock = rock;
            Offset = offset;
        }
    }

    public class CurrentRock2
    {
        /*
         * Rock within chamber, such as:
         * 0b00001000
         * 0b00011100
         * 0b00001000
         */
        public byte[] bytes { get; set; }

        // Y offset from the top of chamber
        public int YOffset { get; set; }

        public CurrentRock2(byte[] bytes, int yOffset)
        {
            this.bytes = bytes;
            YOffset = yOffset;
        }
    }

    /*
     * This rock will only need to be instantiated once. The Bytes array size is set to the biggest rock and array is
     * updated with each new rock from top-to-bottom, so the last unused rows are omitted for smaller rocks.
     */
    public class CurrentRock3
    {
        /*
         * Rock within chamber, such as:
         * 0b00001000
         * 0b00011100
         * 0b00001000
         * 0b00000000
         */
        public byte[] Bytes { get; set; }

        // Y offset from the top of chamber
        public int YOffset { get; set; }

        /*
         * True if the instance holds consistent info.
         * Should be true between IChamber.SpawnRock and IChamber.PutRockToSleep, false otherwise
         */
        public bool Valid { get; set; }

        /*
         * The byte[] Bytes size will remain the same the whole time, but the rock height will change with each
         * new rock. Only the first [Height] bytes in [Bytes] will be taken into account in calculations.
         */
        public int Height { get; set; }

        public CurrentRock3(int highestRock)
        {
            Bytes = new byte[highestRock];
            Valid = false;
        }
    }

    /*
     * This rock is represented by int value, which simplifies left and right moves to a simple shift. Rocks smaller
     * in height will have the top bytes equal to 0, which is not a problem when the rock is compared with parts of
     * chamber by bitwise 'and'.
     */
    public class CurrentRock4
    {
        /*
         * Rock within chamber represented by 32 bits, such as:
         * 00000000
         * 00001000
         * 00011100
         * 00001000
         */
        public int Value { get; set; }

        /*
         * Part of chamber around current rock. The "row" where the lowest part of rock currently is should be the
         * lowest byte of this int, and so on. This way the Value can be bitwise compared with ChamberContext.
         *
         * rock:       chamber context:
         * 00000000 -> byte 4
         * 00001000 -> byte 3
         * 00011100 -> byte 2
         * 00001000 -> byte 1       --> int32 = byte4 byte3 byte2 byte1
         */
        public int ChamberContext { get; set; }

        // Y offset from the top of chamber
        public int YOffset { get; set; }

        /*
         * True if the instance holds consistent info.
         * Should be true between IChamber.SpawnRock and IChamber.PutRockToSleep, false otherwise
         */
        public bool Valid { get; set; }

        /*
         * The byte[] Bytes size will remain the same the whole time, but the rock height will change with each
         * new rock. Only the first [Height] bytes in [Bytes] will be taken into account in calculations.
         */
        public int Height { get; set; }

        public CurrentRock4()
        {
            Valid = false;
        }
    }
}
