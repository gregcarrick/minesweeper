using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for GameEndMessageWindow.xaml
    /// </summary>
    public partial class GameEndMessageWindow : Window
    {
        private delegate void RestartDelegate();

        private RestartDelegate restartDelegate;

        public GameEndMessageWindow(bool win, MainWindow window)
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Title = win ?
                "Congratulations!" :
                "Game Over";

            this.text.Text = string.Format("You've {0} the game.\r\n\r\nPlay again?", win ? "won" : "lost");
            this.text.FontFamily = new FontFamily("Consolas");
            this.text.FontSize = 16;

            this.quitButton.Click += quitButton_Click;

            this.restartButton.Click += restartButton_Click;

            this.restartDelegate = window.Reset;
        }

        private void restartButton_Click(object sender, MouseEventArgs e)
        {
            if (this.restartDelegate != null)
            {
                this.restartDelegate.DynamicInvoke();
            }

            this.Close();
        }

        private void quitButton_Click(object sender, MouseEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.restartButton != null)
            {
                this.restartButton.Click -= restartButton_Click;
            }
            if (this.quitButton != null)
            {
                this.quitButton.Click -= quitButton_Click;
            }
            base.OnClosed(e);
        }
    }
}
