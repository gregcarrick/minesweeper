using Minesweeper.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Minesweeper
{
    /// <summary>
    /// Provides a singleton interface that windows can use to interact with the
    /// game difficulty settings.
    /// </summary>
    public class DifficultyModel : INotifyPropertyChanged
    {
        private bool isSessionActive;
        private Window window;
        private int columns;
        private int rows;
        private int mines;
        private Difficulty difficulty;

        public event PropertyChangedEventHandler PropertyChanged;

        private DifficultyModel() { }

        public static DifficultyModel Instance { get; } = new DifficultyModel();

        public bool IsActiveSession { get; private set; }

        public Difficulty Difficulty
        {
            get
            {
                return this.difficulty;
            }
            set
            {
                if (value != this.difficulty)
                {
                    this.difficulty = value;
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
                return this.columns;
            }
            set
            {
                if (value != this.columns)
                {
                    this.columns = Math.Max(8, Math.Min(30, value));
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
                return this.rows;
            }
            set
            {
                if (value != this.rows)
                {
                    this.rows = Math.Max(8, Math.Min(30, value));
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
                return this.mines;
            }
            set
            {
                if (value != this.mines)
                {
                    this.mines =
                        Math.Max(10, Math.Min((this.Columns - 1) * (this.Rows - 1), value));

                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Reads the settings to start a settings session for a window.
        /// </summary>
        /// <param name="window">The window calling StartSession. The same window
        /// must call <see cref="EndSession(Window)"/> in order to write the
        /// settings values.
        /// </param>
        public void StartSession(Window window)
        {
            if (!this.isSessionActive && this.window == null)
            {
                this.isSessionActive = true;
                this.window = window;
                this.window.Closed += window_Closed;
                this.Columns = Settings.Default.Columns;
                this.Rows = Settings.Default.Rows;
                this.Mines = Settings.Default.Mines;
                this.Difficulty = Settings.Default.Difficulty;
            }
        }

        /// <summary>
        /// Ends the active session by writing the settings.
        /// </summary>
        /// <param name="window">The window calling EndSession. Must be the same
        /// window that previously called <see cref="StartSession(Window)"/>
        /// </param>
        public void EndSession(Window window)
        {
            if (window.GetHashCode() == this.window.GetHashCode())
            {
                Settings.Default.Columns = this.Columns;
                Settings.Default.Rows = this.Rows;
                Settings.Default.Mines = this.Mines;
                Settings.Default.Difficulty = this.Difficulty;
                Settings.Default.Save();
                this.window.Closed -= window_Closed;
                this.window = null;
                this.isSessionActive = false;
            }
        }

        /// <summary>
        /// Makes sure any active session is ended and this.window is set to null.
        /// </summary>
        private void window_Closed(object sender, EventArgs e)
        {
            this.isSessionActive = false;
            this.window = null;
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
