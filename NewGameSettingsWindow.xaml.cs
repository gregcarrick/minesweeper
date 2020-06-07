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

            this.Closing += newGameSettingsWindow_Closing;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    StartGame();
                    break;
                case Key.Escape:
                    Cancel();
                    break;
            }

            base.OnKeyDown(e);
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

        private void newGameSettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            // Prevents the player from closing the window.
            e.Cancel = true;
        }

        private void closeAndStartNewGameButton_Click(object sender, MouseEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            DifficultyModel.Instance.EndSession(this);
            this.NewGameButtonClick?.Invoke(this, new EventArgs());
            this.Closing -= newGameSettingsWindow_Closing;
            Close();
        }

        private void cancelButton_Click(object sender, MouseEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            this.CancelButtonClick?.Invoke(this, new EventArgs());
            this.Closing -= newGameSettingsWindow_Closing;
            Close();
        }
    }
}
