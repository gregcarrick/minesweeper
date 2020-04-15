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

        public NewGameSettingsWindow(Action restartDelegate)
        {
            InitializeComponent();

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
            Close();
        }
    }
}
