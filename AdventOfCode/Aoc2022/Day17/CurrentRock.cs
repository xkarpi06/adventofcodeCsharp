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
}
