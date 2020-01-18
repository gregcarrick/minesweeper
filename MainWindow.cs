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
        private bool[,] minefield;
        private bool gameStarted;
        private int spareMineCell;
        private Dictionary<int, Dictionary<int, MinesweeperButton>> buttons;
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

            // Get a random index from the remaining unmined cells. We will move
            // a mine here if the user's first click happens to be on one of the
            // mined cells.
            this.spareMineCell = indices[rand.Next(this.numMines, n)];
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
                        if (this.gameStarted)
                        {
                            LoseGame();
                        }
                        else
                        {
                            // Ensure the first click is safe by moving the mine
                            // to a free cell, then open the cell that was clicked.
                            int newX = this.spareMineCell % this.width;
                            int newY = this.spareMineCell / this.width;
                            this.minefield[x, y] = false;
                            this.minefield[newX, newY] = true;
                            (this.buttons[x])[y].Open();
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
        private void LoseGame()
        {
            Dictionary<int, MinesweeperButton> column;
            MinesweeperButton button;
            for (int i = 0; i < this.width; i++)
            {
                column = this.buttons[i];
                for (int j = 0; j < this.height; j++)
                {
                    button = column[j];
                    if (IsMine(i, j))
                    {
                        button.Detonate();
                    }
                    else
                    {
                        button.Freeze();
                    }

                    button.StateChanged -= button_StateChanged;
                }
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
            Dictionary<int, MinesweeperButton> column;
            MinesweeperButton button;
            for (int i = 0; i < this.width; i++)
            {
                column = this.buttons[i];
                for (int j = 0; j < this.height; j++)
                {
                    button = column[j];
                    button.Freeze();
                    button.StateChanged -= button_StateChanged;
                }
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
