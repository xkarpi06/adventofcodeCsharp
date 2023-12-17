using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Aoc2022.Day17
{
    /*
      Rock      #### | .#. | ..# | # | ##
      shapes:        | ### | ..# | # | ##
                     | .#. | ### | # | 
                     |     |     | # | 
    */
    public enum Rock { MINUS, PLUS, L, I, SQUARE }

    static class RockExtensions
    {
        private static readonly HashSet<XY> MinusCoords = new HashSet<XY> { new(0, 0), new(1, 0), new(2, 0), new(3, 0) };
        private static readonly HashSet<XY> PlusCoords = new HashSet<XY> { new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2) };
        private static readonly HashSet<XY> LCoords = new HashSet<XY> { new(2, 0), new(2, 1), new(2, 2), new(1, 2), new(0, 2) };
        private static readonly HashSet<XY> ICoords = new HashSet<XY> { new(0, 0), new(0, 1), new(0, 2), new(0, 3) };
        private static readonly HashSet<XY> SquareCoords = new HashSet<XY> { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };

        private static readonly byte[] MinusCoordsByteArray = [
            0b11110000, // ####
        ];
        private static readonly byte[] PlusCoordsByteArray = [
            0b01000000, // .#.
            0b11100000, // ###
            0b01000000, // .#.
        ];
        private static readonly byte[] LCoordsByteArray = [
            0b00100000, // ..#
            0b00100000, // ..#
            0b11100000, // ###
        ];
        private static readonly byte[] ICoordsByteArray = [
            0b10000000, // #
            0b10000000, // #
            0b10000000, // #
            0b10000000, // #
        ];
        private static readonly byte[] SquareCoordsByteArray = [
            0b11000000, // ##   
            0b11000000, // ##
        ];

        /*
         * Returns set of 2D coordinates representing each pixel of the shape offset from top left corner
         * Each shape has origin (0,0) in top-left corner
         */
        public static HashSet<XY> GetCoords(this Rock rock)
        {
            return rock switch
            {
                Rock.MINUS => MinusCoords,
                Rock.PLUS => PlusCoords,
                Rock.L => LCoords,
                Rock.I => ICoords,
                Rock.SQUARE => SquareCoords,
                _ => new HashSet<XY>()
            };
        }

        /*
         * Returns byte array representing coordinates of each pixel of the shape as if spawned in top left corner
         */
        public static byte[] GetCoordsByteArray(this Rock rock)
        {
            return rock switch
            {
                Rock.MINUS => [
                    0b11110000, // ####
                ],
                Rock.PLUS => [
                    0b01000000, // .#.
                    0b11100000, // ###
                    0b01000000, // .#.
                ],
                Rock.L => [
                    0b00100000, // ..#
                    0b00100000, // ..#
                    0b11100000, // ###
                ],
                Rock.I => [
                    0b10000000, // #
                    0b10000000, // #
                    0b10000000, // #
                    0b10000000, // #
                ],
                Rock.SQUARE => [
                    0b11000000, // ##   
                    0b11000000, // ##
                ],
                _ => []
            };
        }

        /*
         * Returns byte array representing coordinates of each pixel of the shape as if spawned in top left corner
         */
        public static byte[] GetCoordsByteArray3(this Rock rock)
        {
            return rock switch
            {
                Rock.MINUS => MinusCoordsByteArray,
                Rock.PLUS => PlusCoordsByteArray,
                Rock.L => LCoordsByteArray,
                Rock.I => ICoordsByteArray,
                Rock.SQUARE => SquareCoordsByteArray,
                _ => []
            };
        }

        /*
         * Each rock is represented by a single integer value, where 1 represents rock piece, 0 represents void.
         * The integers are hard-coded for chamber WIDTH = 7 and LEFT_OFFSET = 2
         */
        public static int GetIntValueFixedWidth7Offset2(this Rock rock)
        {
            return rock switch
            {
                // ..####
                Rock.MINUS => 30,     // 00000000 00000000 00000000 00011110
                // ...#.
                // ..###
                // ...#.
                Rock.PLUS => 531_464, // 00000000 00001000 00011100 00001000
                // ....#
                // ....#
                // ..###
                Rock.L => 263_196,    // 00000000 00000100 00000100 00011100
                // ..#
                // ..#
                // ..#
                // ..#
                Rock.I => 269_488_144,// 00010000 00010000 00010000 00010000
                // ..##
                // ..##
                Rock.SQUARE => 6_168, // 00000000 00000000 00011000 00011000
                _ => 0
            };
        }

        /*
         * Hard coded because the shapes will hardly ever change
         */
        public static int GetHeight(this Rock rock)
        {
            switch (rock)
            {
                case Rock.MINUS: return 1;
                case Rock.PLUS: return 3;
                case Rock.L: return 3;
                case Rock.I: return 4;
                case Rock.SQUARE: return 2;
                default: return 0;
            }
        }

        /*
         * Hard coded because the shapes will hardly ever change
         */
        public static int GetWidth(this Rock rock)
        {
            switch (rock)
            {
                case Rock.MINUS: return 4;
                case Rock.PLUS: return 3;
                case Rock.L: return 3;
                case Rock.I: return 1;
                case Rock.SQUARE: return 2;
                default: return 0;
            }
        }

        public static List<Rock> GetOrderedList()
        {
            return new List<Rock> { Rock.MINUS, Rock.PLUS, Rock.L, Rock.I, Rock.SQUARE };
        }
    }
}
