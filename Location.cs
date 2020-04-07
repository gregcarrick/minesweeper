namespace Minesweeper
{
    public struct Location
    {
        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj != null && obj is Location)
            {
                Location loc = (Location)obj;
                result = this.X == loc.X && this.Y == loc.Y;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.X, this.Y);
        }

        public static bool operator ==(Location loc1, Location loc2)
        {
            return loc1.X == loc2.X && loc1.Y == loc2.Y;
        }

        public static bool operator !=(Location loc1, Location loc2)
        {
            return loc1.X != loc2.X || loc1.Y != loc2.Y;
        }
    }
}
