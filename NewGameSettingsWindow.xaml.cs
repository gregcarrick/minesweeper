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
        public NewGameSettingsWindow()
        {
            InitializeComponent();

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

        private void closeButton_Click(object sender, MouseEventArgs e)
        {
            this.Closing -= newGameSettingsWindow_Close;
            Close();
        }

        private void newGameSettingsWindow_Close(object sender, CancelEventArgs e)
        {
            // Prevents the player from closing the window.
            e.Cancel = true;
        }

        private void cancelButton_Click(object sender, MouseEventArgs e)
        {
            this.Closing -= newGameSettingsWindow_Close;
            Close();
        }
    }
}
