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
}
