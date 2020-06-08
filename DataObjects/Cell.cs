namespace Minesweeper
{
    public struct Cell
    {
        public Cell(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.IsMine = false;
            this.AdjacentMines = 0;
            this.State = CellState.Default;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public bool IsMine { get; set; }

        public int AdjacentMines { get; set; }

        public CellState State { get; set; }
    }
}
