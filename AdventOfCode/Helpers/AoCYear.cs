namespace AdventOfCode.Helpers
{
    internal enum AoCYear
    {
        AOC_2022,
        AOC_2023
    }

    internal static class AoCYearExtensions
    {
        public static string GetResourcePath(this AoCYear year)
        {
            switch (year)
            {
                case AoCYear.AOC_2022: return "Resources.Aoc2022";
                case AoCYear.AOC_2023: return "Resources.Aoc2023";
                default: throw new ArgumentOutOfRangeException(nameof(year), year, null);
            }
        }
    }

}
