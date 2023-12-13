namespace AdventOfCode.Helpers
{
    public class XY
    {
        public int X { get; set; }
        public int Y { get; set; }

        public XY(int x, int y) => (X, Y) = (x, y);

        public override string ToString() => $"({X}, {Y})";

        public override bool Equals(object obj)
        {
            if (obj is XY other)
            {
                return X == other.X && Y == other.Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
    }

    public enum Dir { UP, DOWN, LEFT, RIGHT }

    public static class DirExtensions
    {
        public static Dir Opposite(this Dir dir)
        {
            switch (dir)
            {
                case Dir.UP: return Dir.DOWN;
                case Dir.DOWN: return Dir.UP;
                case Dir.LEFT: return Dir.RIGHT;
                default: return Dir.LEFT;
            }
        }
    }
}
