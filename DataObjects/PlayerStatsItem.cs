using System;

namespace Minesweeper
{
    [Serializable]
    public class PlayerStatsItem
    {
        public PlayerStatsItem()
        {
            this.Date = null;
            this.Fastest = null;
            this.Plays = 0;
            this.Wins = 0;
        }

        public DateTime? Date { get; set; }
        public int? Fastest { get; set; }
        public int Plays { get; set; }
        public int Wins { get; set; }
    }
}
