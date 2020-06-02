using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model model;

        public MainWindow()
        {
            InitializeComponent();

            Reset();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public static readonly DependencyProperty TimerValueProperty =
            DependencyProperty.Register("TimerValue", typeof(int), typeof(MainWindow),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None));
        public int TimerValue
        {
            get
            {
                return (int)GetValue(TimerValueProperty);
            }
            set
            {
                SetValue(TimerValueProperty, value);
            }
        }

        public static readonly DependencyProperty RemainingMinesProperty =
            DependencyProperty.Register("RemainingMines", typeof(int), typeof(MainWindow),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None));
        public int RemainingMines
        {
            get
            {
                return (int)GetValue(RemainingMinesProperty);
            }
            set
            {
                SetValue(RemainingMinesProperty, value);
            }
        }

        public void Reset()
        {
            this.model = new Model();
            this.model.PropertyChanged += model_PropertyChanged;
            this.model.Reset();

            this.boardView.Model = this.model;
            this.boardView.Reset();

            this.IsEnabled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.newGameButton != null)
            {
                this.newGameButton.Click -= newGameButton_Click;
            }
            base.OnClosing(e);
        }

        private void newGameButton_Click(object sender, MouseEventArgs e)
        {
            Reset();
        }

        private void statsMenuItem_Click(object sender, MouseEventArgs e)
        {
            //var window = new PlayerStatsWindow();
            //window.Owner = GetWindow(this);
            //window.Show();
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TimerValue":
                    this.TimerValue = this.model.TimerValue;
                    break;
                case "RemainingMines":
                    this.RemainingMines = this.model.RemainingMines;
                    break;
                case "State":
                    switch (this.model.State)
                    {
                        case GameState.Lost:
                            EndGame(false);
                            break;
                        case GameState.Won:
                            EndGame(true);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when the model's game state changes to Lost or Won.
        /// </summary>
        private void EndGame(bool win)
        {
            GameEndMessageWindow window = new GameEndMessageWindow(win);
            window.RestartButtonClick += window_RestartButtonClick;
            window.Owner = GetWindow(this); // Pop up over the centre of the main window
            window.Show();

            this.IsEnabled = false;
        }

        private void window_RestartButtonClick(object sender, EventArgs e)
        {
            Reset();
        }

        private void newGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewGameSettingsWindow window = new NewGameSettingsWindow();
            window.CancelButtonClick += window_CancelButtonClick;
            window.NewGameButtonClick += window_NewGameButtonClick;
            window.Owner = GetWindow(this);
            window.Show();

            this.model.StopTimer();
            this.IsEnabled = false;
        }

        private void window_NewGameButtonClick(object sender, EventArgs e)
        {
            Reset();
        }

        private void window_CancelButtonClick(object sender, EventArgs e)
        {
            this.model.StartTimer();
            this.IsEnabled = true;
        }

        private void statsMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
