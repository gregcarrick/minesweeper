﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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

            this.Closing += gameEndMessageWindow_Closing;

            this.title.Content = win ?
                "Congratulations!" :
                "Game Over :(";

            this.text.Content = string.Format("You've {0} the game.\r\n\r\nPlay again?", win ? "won" : "lost");

            this.quitButton.Click += quitButton_Click;

            this.restartButton.Click += restartButton_Click;

            this.restartDelegate = window.Reset;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.restartButton != null)
            {
                this.restartButton.Click -= restartButton_Click;
            }
            if (this.quitButton != null)
            {
                this.quitButton.Click -= quitButton_Click;
            }
            base.OnClosing(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Restart();
                    break;
                case Key.Escape:
                    Quit();
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

        private void gameEndMessageWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevents the player from closing the popup.
            e.Cancel = true;
        }

        private void restartButton_Click(object sender, MouseEventArgs e)
        {
            Restart();
        }

        private void Restart()
        {
            if (this.restartDelegate != null)
            {
                this.restartDelegate.DynamicInvoke();
            }

            this.Closing -= gameEndMessageWindow_Closing;
            Close();
        }

        private void quitButton_Click(object sender, MouseEventArgs e)
        {
            Quit();
        }

        private void Quit()
        {
            this.Closing -= gameEndMessageWindow_Closing;
            Application.Current.Shutdown();
        }
    }
}
