namespace AdventOfCode.Helpers
{
    public struct XY
    {
        public int X { get; }
        public int Y { get; }

        public XY(int x, int y) => (X, Y) = (x, y);
        
        public override string ToString() => $"({X}, {Y})";
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
