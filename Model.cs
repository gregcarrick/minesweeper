﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Model containing the game state.
    /// </summary>
    public class Model : INotifyPropertyChanged
    {
        private int rowCount;
        private int columnCount;
        private int mines;
        private int remainingMines;
        private int remainingCells;
        private Tuple<int, int> spareMineIndex;
        private GameState state;
        private Cell[,] mineField;
        private DispatcherTimer timer;
        private int timerValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public Model()
        {
        }

        public Cell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= this.columnCount || y < 0 || y >= this.rowCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this.mineField[x, y];
            }
            private set
            {
                if (x < 0 || x >= this.columnCount || y < 0 || y >= this.rowCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.mineField[x, y] = value;
            }
        }

        public int TimerValue
        {
            get
            {
                return this.timerValue;
            }
            private set
            {
                this.timerValue = value;
                NotifyPropertyChanged();
            }
        }

        public int RemainingMines
        {
            get
            {
                return this.remainingMines;
            }
            private set
            {
                this.remainingMines = value;
                NotifyPropertyChanged();
            }
        }

        public GameState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
                NotifyPropertyChanged();
            }
        }

        public void Reset(int rowCount, int columnCount, int totalMines)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.mines = totalMines;
            this.RemainingMines = totalMines;
            this.remainingCells = rowCount * columnCount - totalMines;

            PlaceMines();

            this.TimerValue = 0;
            if (this.timer != null)
            {
                this.timer.Tick -= timer_Tick;
            }
            this.timer = new DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 1); // 1 second
            this.timer.Tick += timer_Tick;

            this.State = GameState.Ready;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.TimerValue++;
        }

        private void PlaceMines()
        {
            this.mineField = new Cell[this.rowCount, this.columnCount];
            for (int y = 0; y < this.rowCount; y++)
            {
                for (int x = 0; x < this.columnCount; x++)
                {
                    this.mineField[x, y] = new Cell(x, y);
                }
            }

            // Use a truncated Fisher-Yates shuffle to randomly place mines in the array.
            // This algorithm selects a set of random numbers without duplicates.
            int n = this.rowCount * this.columnCount;
            Random rand = new Random();
            int[] indices = new int[n];
            int temp;
            for (int i = 0; i < n; i++)
            {
                indices[i] = i;
            }
            for (int i = 0; i < this.mines; i++)
            {
                int j = rand.Next(i, n);

                // Swap the randomly selected value with the i-th value.
                // The first i+1 values are considered sorted and will be
                // ignored on the next pass.
                temp = indices[j];
                indices[j] = indices[i];
                indices[i] = temp;

                // Place a mine at the randomly selected index.
                this.mineField[temp % this.columnCount, temp / this.columnCount].IsMine = true;
            }

            // Get a random index from the remaining unmined cells. We will move
            // a mine here if the user's first click happens to be on one of the
            // mined cells.
            int index = indices[rand.Next(this.mines, n)];
            this.spareMineIndex = Tuple.Create(index % this.columnCount, index / this.columnCount);
        }

        public void FlagCell(int x, int y)
        {
            CellState state = this.mineField[x, y].State;
            switch (state)
            {
                case CellState.Default:
                    this.mineField[x, y].State = CellState.Flagged;
                    this.RemainingMines--;
                    break;
                case CellState.Flagged:
                    this.mineField[x, y].State = CellState.Default;
                    this.RemainingMines++;
                    break;
                default:
                    break;
            }
        }

        public void OpenCell(int x, int y)
        {
            Cell cell = this.mineField[x, y];
            if (cell.State == CellState.Default)
            {
                if (cell.IsMine)
                {
                    if (this.State == GameState.Started)
                    {
                        this.mineField[x, y].State = CellState.Detonated;
                        Lose();
                        return;
                    }
                    else
                    {
                        // Ensure the first click is safe by moving the mine
                        // to a free cell, then open the cell that was clicked.
                        this.mineField[x, y].IsMine = false;
                        this.mineField[this.spareMineIndex.Item1, this.spareMineIndex.Item2].IsMine = true;
                    }
                }

                // We're not going to blow up, so continue opening the cell
                this.mineField[x, y].State = CellState.Opened;
                this.remainingCells--;

                IEnumerable<Tuple<int, int>> adjacentCellCoords = GetAdjacentCellCoords(cell.X, cell.Y);
                int mines = adjacentCellCoords
                            .Where(t => this.mineField[t.Item1, t.Item2].IsMine)
                            .Count();
                this.mineField[x, y].AdjacentMines = mines; // Number to display

                if (mines == 0)
                {
                    // We're inside a clear block, so open the adjacent cells.
                    foreach (var coord in adjacentCellCoords)
                    {
                        OpenCell(coord.Item1, coord.Item2);
                    }
                }

                if (this.state == GameState.Ready)
                {
                    this.state = GameState.Started;
                    this.timer.Start();
                }

                if (this.remainingCells == 0 && this.State == GameState.Started)
                {
                    this.Win();
                }
            }
        }

        private void Win()
        {
            this.timer.Stop();
            for (int y = 0; y < this.rowCount; y++)
            {
                for (int x = 0; x < this.columnCount; x++)
                {
                    if (this.mineField[x, y].IsMine)
                    {
                        this.mineField[x, y].State = CellState.Flagged;
                    }
                }
            }
            this.State = GameState.Won;
        }

        private void Lose()
        {
            this.timer.Stop();
            this.State = GameState.Lost;
        }

        /// <summary>
        /// Returns an enumerable collection of the 1d array indices corresponding
        /// to the coordinates of a given cell's eight adjacent cells.
        /// </summary>
        private IList<Tuple<int, int>> GetAdjacentCellCoords(int x, int y)
        {
            var result = new List<Tuple<int, int>>();
            for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, this.rowCount - 1); j++)
            {
                for (int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, this.columnCount - 1); i++)
                {
                    if (!(i == x && j == y))
                    {
                        result.Add(Tuple.Create(i, j));
                    }
                }
            }

            return result;
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
