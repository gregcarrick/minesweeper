using Minesweeper.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    /// <summary>
    /// Provides a singleton interface for interacting with the game difficulty settings.
    /// </summary>
    public class DifficultyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DifficultyModel() { }

        public static DifficultyModel Instance { get; } = new DifficultyModel();

        public Difficulty Difficulty
        {
            get
            {
                return Settings.Default.Difficulty;
            }
            set
            {
                if (value != Settings.Default.Difficulty)
                {
                    Settings.Default.Difficulty = value;
                    if (value != Difficulty.Custom)
                    {
                        this.Columns = value switch
                        {
                            Difficulty.Beginner => 8,
                            Difficulty.Intermediate => 16,
                            Difficulty.Expert => 30,
                            _ => this.Columns
                        };
                        this.Rows = value switch
                        {
                            Difficulty.Beginner => 8,
                            Difficulty.Intermediate => 16,
                            Difficulty.Expert => 16,
                            _ => this.Rows
                        };
                        this.Mines = value switch
                        {
                            Difficulty.Beginner => 10,
                            Difficulty.Intermediate => 40,
                            Difficulty.Expert => 99,
                            _ => this.Mines
                        };
                    }

                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Number of columns on the board. Minimum = 8, Maximum = 30.
        /// </summary>
        public int Columns
        {
            get
            {
                return Settings.Default.Columns;
            }
            set
            {
                if (value != Settings.Default.Columns)
                {
                    Settings.Default.Columns = Math.Max(8, Math.Min(30, value));
                    this.Mines = Math.Min((this.Columns - 1) * (this.Rows - 1), this.Mines);

                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Number of rows on the board. Minimum = 8, Maximum = 30.
        /// </summary>
        public int Rows
        {
            get
            {
                return Settings.Default.Rows;
            }
            set
            {
                if (value != Settings.Default.Rows)
                {
                    Settings.Default.Rows = Math.Max(8, Math.Min(30, value));
                    this.Mines = Math.Min((this.Columns - 1) * (this.Rows - 1), this.Mines);

                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Number of mines on the board. Minimum = 10, Maximum = (Columns - 1)(Rows - 1).
        /// </summary>
        public int Mines
        {
            get
            {
                return Settings.Default.Mines;
            }
            set
            {
                if (value != Settings.Default.Mines)
                {
                    Settings.Default.Mines =
                        Math.Max(10, Math.Min((this.Columns - 1) * (this.Rows - 1), value));

                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool IsCustom => this.Difficulty == Difficulty.Custom;
    }
}
