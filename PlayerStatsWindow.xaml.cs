using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for PlayerStatsWindow.xaml
    /// </summary>
    public partial class PlayerStatsWindow : Window
    {
        public event EventHandler CloseButtonClick;

        public PlayerStatsWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Closing += playerStatsWindow_Closing;

            for (int i = 0; i < 4; i++)
            {
                Difficulty difficulty = (Difficulty)i;
                PlayerStatsItem statsItem = PlayerStatsModel.Stats[difficulty];
                // Get all the stats strings in the order they'll be displayed in the row.
                IList<string> strings = new List<string>()
                {
                    Enum.GetName(typeof(Difficulty), i), // Difficulty
                    statsItem.Plays.ToString(), // Plays
                    statsItem.Wins.ToString(), // Wins
                    GetWinPercentage(statsItem), // Ratio
                    FormatFastestTime(difficulty, statsItem.Fastest), // Fastest time
                    FormatDate(difficulty, statsItem.Date) // Date
                };

                for (int j = 0; j < strings.Count; j++)
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = strings[j],
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(6),
                    };
                    Grid.SetRow(textBlock, i + 1); // Row 0 contains the columns headers so skip it.
                    Grid.SetColumn(textBlock, j);
                    this.statsGrid.Children.Add(textBlock);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point hit = e.GetPosition(null);
            if (hit.X >= 0 && hit.X < this.title.ActualWidth && hit.Y >= 0 && hit.Y < this.title.ActualHeight)
            {
                this.Cursor = Cursors.SizeAll;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private string GetWinPercentage(PlayerStatsItem statsItem)
        {
            return statsItem.Plays == 0
                ? "-"
                : Math.Round((100.0 * statsItem.Wins) / statsItem.Plays, 0).ToString() + "%";
        }

        private string FormatFastestTime(Difficulty difficulty, int? time)
        {
            if (difficulty == Difficulty.Custom)
            {
                return "N/A";
            }
            else if (time == null)
            {
                return "-";
            }
            else
            {
                return time.ToString() + "s";
            }
        }

        private string FormatDate(Difficulty difficulty, DateTime? date)
        {
            if (difficulty == Difficulty.Custom)
            {
                return "N/A";
            }
            else if (date == null)
            {
                return "-";
            }
            else
            {
                return date.Value.ToLocalTime().ToString();
            }
        }

        private void playerStatsWindow_Closing(object sender, CancelEventArgs e)
        {
            // Prevents the player from closing the window.
            e.Cancel = true;
        }

        private void closeButton_Click(object sender, MouseEventArgs e)
        {
            this.CloseButtonClick?.Invoke(this, new EventArgs());
            this.Closing -= playerStatsWindow_Closing;
            this.Close();
        }
    }
}
