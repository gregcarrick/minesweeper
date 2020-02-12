using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class MainWindow : Form
    {
        private int width;
        private int height;
        private int remainingCells;
        private int numMines;
        private int remainingMines;
        private bool[] minefield;
        private bool gameStarted;
        private int spareMineIndex;
        private MinesweeperButton[] buttons;
        private Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            this.width = 8;
            this.height = 8;
            this.mineCounter.Value = remainingMines = numMines = 10;
            this.remainingCells = this.width * this.height - this.numMines;

            this.panel1.Paint += panel1_Paint;

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += Timer_Tick;

            PlaceMines();
            CreateButtons();
        }

        /// <summary>
        /// Add shading around frame
        /// </summary>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Shade outside bottom and right
            e.Graphics.DrawLines(
                Pens.Gray,
                new Point[]
                {
                    new Point(1,                        this.panel1.Height - 1  ),
                    new Point(this.panel1.Width - 1,    this.panel1.Height - 1  ),
                    new Point(this.panel1.Width - 1,    1                       ),
                });
            // Shade inside top and left
            e.Graphics.DrawLines(
                Pens.Gray,
                new Point[]
                {
                    new Point(2,                        this.panel1.Height - 4  ),
                    new Point(2,                        2                       ),
                    new Point(this.panel1.Width - 4,    2                       ),
                });
            // Highlight outside top and left
            e.Graphics.DrawLines(
                Pens.WhiteSmoke,
                new Point[]
                {
                    new Point(0,                        this.panel1.Height - 2  ),
                    new Point(0,                        0                       ),
                    new Point(this.panel1.Width - 2,    0                       ),
                });
            // Hightlight inside bottom and right
            e.Graphics.DrawLines(
                Pens.WhiteSmoke,
                new Point[]
                {
                    new Point(3,                        this.panel1.Height - 3  ),
                    new Point(this.panel1.Width - 3,    this.panel1.Height - 3  ),
                    new Point(this.panel1.Width - 3,    3                       ),
                });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timerControl.Value++;
        }

        private void PlaceMines()
        {
            this.minefield = new bool[this.height * this.width];

            // Use a truncated Fisher-Yates shuffle to randomly place mines in the array.
            // This algorithm selects a set of random numbers without duplicates.
            int n = this.height * this.width;
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

                // Place a mine at the randomly selected index.
                this.minefield[temp] = true;
            }

            // Get a random index from the remaining unmined cells. We will move
            // a mine here if the user's first click happens to be on one of the
            // mined cells.
            this.spareMineIndex = indices[rand.Next(this.numMines, n)];
        }

        private void CreateButtons()
        {
            this.panel.SuspendLayout();

            this.buttons = new MinesweeperButton[this.height * this.width];
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    MinesweeperButton button = new MinesweeperButton(x, y);
                    button.Location = new Point(x * 24, y * 24);
                    button.Margin = new Padding(0);
                    button.Padding = new Padding(0);
                    button.Size = new Size(24, 24);
                    panel.Controls.Add(button);

                    button.StateChanged += button_StateChanged;

                    this.buttons[Get1dArrayIndex(x, y)] = button;
                }
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
                    int index = Get1dArrayIndex(x, y);
                    if (IsMine(index))
                    {
                        if (this.gameStarted)
                        {
                            LoseGame();
                        }
                        else
                        {
                            // Ensure the first click is safe by moving the mine
                            // to a free cell, then open the cell that was clicked.
                            this.minefield[index] = false;
                            this.minefield[this.spareMineIndex] = true;
                            this.buttons[index].Open();
                        }
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

                        this.remainingCells--;

                        if (this.remainingCells == 0)
                        {
                            WinGame();
                        }
                    }

                    // Each cell can only be opened once, so detach the event handler.
                    button.StateChanged -= button_StateChanged;

                    if (!this.gameStarted)
                    {
                        this.gameStarted = true;
                        this.timer.Start();
                    }
                    break;
            }
        }

        /// <summary>
        /// Counts the number of mines adjacent to a given cell, including diagonals.
        /// </summary>
        private int CalculateAdjacentMines(int x, int y)
        {
            int mines = 0;
            
            foreach (int index in GetAdjacentCellIndices(x, y))
            {
                if (IsMine(index))
                {
                    mines++;
                }
            }

            return mines;
        }

        /// <summary>
        /// Checks whether a given cell is a mine.
        /// </summary>
        private bool IsMine(int index)
        {
            return this.minefield[index];
        }

        /// <summary>
        /// Opens all adjactent cells to a given cell, including diagonals.
        /// </summary>
        private void OpenAdjacentCells(int x, int y)
        {
            MinesweeperButton button;
            foreach (int index in GetAdjacentCellIndices(x, y))
            {
                button = this.buttons[index];
                if (button.State == ButtonState.Default)
                {
                    button.Open();
                }
            }
        }

        /// <summary>
        /// Returns an enumerable collection of the 1d array indices corresponding
        /// to the coordinates of a given cell's eight adjacent cells.
        /// </summary>
        private IEnumerable<int> GetAdjacentCellIndices(int x, int y)
        {
            for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, this.height - 1); j++)
            {
                for (int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, this.width - 1); i++)
                {
                    if (!(i == x && j == y))
                    {
                        yield return Get1dArrayIndex(i, j);
                    }
                }
            }
        }

        private int Get1dArrayIndex(int x, int y)
        {
            return y * this.width + x;
        }

        /// <summary>
        /// Reveals all the mines, ending the game.
        /// </summary>
        private void LoseGame()
        {
            MinesweeperButton button;
            for (int index = 0; index < this.height * this.width; index++)
            {
                button = this.buttons[index];
                if (IsMine(index))
                {
                    button.Detonate();
                }
                else
                {
                    button.Freeze();
                }

                button.StateChanged -= button_StateChanged;
            }

            this.timer.Stop();

            if (MessageBox.Show(
                this,
                "You have lost the game.",
                ":-(",
                MessageBoxButtons.OK) == DialogResult.OK)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Ends the game and shows a nice message.
        /// </summary>
        private void WinGame()
        {
            MinesweeperButton button;
            for (int index = 0; index < this.height * this.width; index++)
            {
                button = this.buttons[index];
                if (this.minefield[index] && button.State == ButtonState.Default)
                {
                    button.Flag();
                }
                button.Freeze();
                button.StateChanged -= button_StateChanged;
            }

            this.timer.Stop();

            if (MessageBox.Show(
                this,
                "Congratulations! You have won.",
                ":-)",
                MessageBoxButtons.OK) == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
