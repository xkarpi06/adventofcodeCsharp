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
}
