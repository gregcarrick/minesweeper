using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private int width;
        private int height;
        private int numMines;
        private int remainingMines;
        private bool[,] minefield;
        private Dictionary<int, Dictionary<int, MinesweeperButton>> buttons;

        public Form1()
        {
            InitializeComponent();

            width = 8;
            height = 8;
            this.mineCounter.Value = remainingMines = numMines = 10;

            PlaceMines();
            CreateButtons();
        }

        private void PlaceMines()
        {
            this.minefield = new bool[this.width, this.height];

            // Use a truncated Fisher-Yates shuffle to randomly place mines in the array.
            // This algorithm selects a set of random numbers without duplicates.
            int n = this.width * this.height;
            Random rand = new Random();
            int[] indices = new int[n];
            int temp;
            for (int i = 0; i < n; i ++)
            {
                indices[i] = i;
            }
            for (int i = 0; i < this.numMines; i++)
            {
                int j = rand.Next(i, n);

                // Swap the randomly selected value with the i-th value.
                // The first i+1 values are considered sorted and will be
                // ignored on the next pass.
                temp = indices[j];
                indices[j] = indices[i];
                indices[i] = temp;

                // Convert the randomly selected value into coordinates in the
                // 2d array, which are used to place a mine.
                int x = temp % this.width;
                int y = temp / this.width;
                this.minefield[x, y] = true;
            }
        }

        private void CreateButtons()
        {
            this.panel.SuspendLayout();

            this.buttons = new Dictionary<int, Dictionary<int, MinesweeperButton>>();
            for (int i = 0; i < this.width; i++)
            {
                Dictionary<int, MinesweeperButton> column = new Dictionary<int, MinesweeperButton>();
                for (int j = 0; j < this.height; j++)
                {
                    MinesweeperButton button = new MinesweeperButton(i, j);
                    button.Location = new Point(i * 24, j * 24);
                    button.Margin = new Padding(0);
                    button.Padding = new Padding(0);
                    button.Size = new Size(24, 24);
                    panel.Controls.Add(button);

                    column.Add(j, button);

                    button.StateChanged += button_StateChanged;
                }

                this.buttons.Add(i, column);
            }

            this.panel.ResumeLayout();
        }

        private void button_StateChanged(object sender, ButtonStateEventArgs e)
        {
            MinesweeperButton button = sender as MinesweeperButton;
            int x = button.X;
            int y = button.Y;

            switch (e.NewState)
            {
                case ButtonState.Default:
                    if (e.OldState == ButtonState.Flagged
                        && this.mineCounter != null)
                    {
                        this.mineCounter.Value = ++this.remainingMines;
                    }
                    break;
                case ButtonState.Flagged:
                    if (this.mineCounter != null)
                    {
                        this.mineCounter.Value = --this.remainingMines;
                    }
                    break;
                case ButtonState.Opened:
                    if (IsMine(x, y))
                    {
                        DetonateMines();
                    }
                    else
                    {
                        int adjacentMines = CalculateAdjacentMines(x, y);
                        if (adjacentMines == 0)
                        {
                            OpenAdjacentCells(x, y);
                        }
                        else
                        {
                            button.SetMinesNumber(adjacentMines);
                        }
                    }

                    // Each cell can only be opened once, so detach the event handler.
                    button.StateChanged -= button_StateChanged;
                    break;
            }
        }

        /// <summary>
        /// Checks whether a given cell is a mine.
        /// </summary>
        private bool IsMine(int x, int y)
        {
            return this.minefield[x, y];
        }

        /// <summary>
        /// Counts the number of mines adjacent to a given cell, including diagonals.
        /// </summary>
        private int CalculateAdjacentMines(int x, int y)
        {
            int mines = 0;
            
            for (int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, this.width - 1); i++)
            {
                for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, this.height - 1); j++)
                {
                    // Exclude the centre square.
                    if (!(i == x && j == y)
                        && IsMine(i, j))
                    {
                        mines++;
                    }
                }
            }

            return mines;
        }

        /// <summary>
        /// Opens all adjactent cells to a given cell, including diagonals.
        /// </summary>
        private void OpenAdjacentCells(int x, int y)
        {
            Dictionary<int, MinesweeperButton> column;
            MinesweeperButton button;
            for (int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, this.width - 1); i++)
            {
                column = this.buttons[i];
                for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, this.height - 1); j++)
                {
                    // Exclude the centre square.
                    if (!(i == x && j == y))
                    {
                        button = column[j];
                        if (button.State == ButtonState.Default)
                        {
                            button.Open();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reveals all the mines, ending the game.
        /// </summary>
        private void DetonateMines()
        {
            Dictionary<int, MinesweeperButton> column;
            MinesweeperButton button;
            for (int i = 0; i < this.width; i++)
            {
                column = this.buttons[i];
                for (int j = 0; j < this.height; j++)
                {
                    button = column[j];
                    button.Detonate(IsMine(i, j));

                    button.StateChanged -= button_StateChanged;
                }
            }
        }
    }
}
