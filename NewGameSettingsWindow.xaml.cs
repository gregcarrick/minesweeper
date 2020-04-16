using Minesweeper.Properties;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for NewGameSettingsWindow.xaml
    /// </summary>
    public partial class NewGameSettingsWindow : Window
    {
        private Action restartDelegate;
        (int columns, int rows, int mines, Difficulty difficulty) cachedSettings;

        public NewGameSettingsWindow(Action restartDelegate)
        {
            InitializeComponent();

            this.cachedSettings = (
                DifficultyModel.Instance.Columns,
                DifficultyModel.Instance.Rows,
                DifficultyModel.Instance.Mines,
                DifficultyModel.Instance.Difficulty);

            this.restartDelegate = restartDelegate;

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Closing += newGameSettingsWindow_Close;
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

        private void newGameSettingsWindow_Close(object sender, CancelEventArgs e)
        {
            // Prevents the player from closing the window.
            e.Cancel = true;
        }

        private void closeAndStartNewGameButton_Click(object sender, MouseEventArgs e)
        {
            this.Closing -= newGameSettingsWindow_Close;
            this.restartDelegate.DynamicInvoke();
            Close();
        }

        private void cancelButton_Click(object sender, MouseEventArgs e)
        {
            this.Closing -= newGameSettingsWindow_Close;
            // Put settings back to their previous values.
            DifficultyModel.Instance.Columns = this.cachedSettings.columns;
            DifficultyModel.Instance.Rows = this.cachedSettings.rows;
            DifficultyModel.Instance.Mines = this.cachedSettings.mines;
            DifficultyModel.Instance.Difficulty = this.cachedSettings.difficulty;
            Close();
        }
    }
}
