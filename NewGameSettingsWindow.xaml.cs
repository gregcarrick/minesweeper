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
        public event EventHandler NewGameButtonClick;
        public event EventHandler CancelButtonClick;

        public NewGameSettingsWindow()
        {
            InitializeComponent();

            DifficultyModel.Instance.StartSession(this);

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
            DifficultyModel.Instance.EndSession(this);
            this.NewGameButtonClick?.Invoke(this, new EventArgs());
            this.Closing -= newGameSettingsWindow_Close;
            Close();
        }

        private void cancelButton_Click(object sender, MouseEventArgs e)
        {
            this.CancelButtonClick?.Invoke(this, new EventArgs());
            this.Closing -= newGameSettingsWindow_Close;
            Close();
        }
    }
}
