using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Minesweeper
{
    /// <summary>
    /// Provides a singleton interface for reading and writing the game statistics.
    /// </summary>
    public class PlayerStatsModel
    {
        private PlayerStatsModel()
        {
            Stats = new Dictionary<Difficulty, PlayerStatsItem>()
            {
                {Difficulty.Beginner, new  PlayerStatsItem() },
                {Difficulty.Intermediate, new  PlayerStatsItem() },
                {Difficulty.Expert, new  PlayerStatsItem() },
                {Difficulty.Custom, new  PlayerStatsItem() },
            };
            if (!File.Exists(GetStatsPath()))
            {
                WriteStats();
            }

            ReadStats();
        }

        public static PlayerStatsModel Instance { get; private set; } = new PlayerStatsModel();

        public static IDictionary<Difficulty, PlayerStatsItem> Stats { get; private set; }

        public void RecordWin(Difficulty difficulty, int time)
        {
            RecordResult(difficulty, time, true);
        }

        public void RecordLoss(Difficulty difficulty, int time)
        {
            RecordResult(difficulty, time, false);
        }

        private void RecordResult(Difficulty difficulty, int time, bool win)
        {
            PlayerStatsItem item = Stats.Keys.Contains(difficulty) ? Stats[difficulty] : new PlayerStatsItem();

            item.Plays++;
            if (win)
            {
                item.Wins++;
                if (item.Fastest == null || item.Fastest > time)
                {
                    item.Fastest = time;
                    item.Date = DateTime.Now.ToUniversalTime();
                }
            }

            Stats[difficulty] = item;
            WriteStats();
        }


        /// <summary>
        /// Deserializes the stats.xml file, first creating it if it is absent.
        /// Stores the data in <see cref="Stats"/>.
        /// </summary>
        private void ReadStats()
        {
            using var fs = File.OpenRead(GetStatsPath());
            BinaryFormatter bf = new BinaryFormatter();
            Stats = bf.Deserialize(fs) as Dictionary<Difficulty, PlayerStatsItem>;
        }


        private void WriteStats()
        {
            if (Stats != null)
            {
                using var fs = File.Open(GetStatsPath(), FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Stats);
            }
        }

        private string GetStatsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Minesweeper\\stats.";
        }
    }
}
