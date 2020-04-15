using Minesweeper.Properties;
using System;
using System.ComponentModel;
using System.Windows;

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

        public int RowCount
        {
            get
            {
                return 8;
            }
        }

        public int ColumnCount
        {
            get
            {
                return 8;
            }
        }

        public int Mines
        {
            get
            {
                return 10;
            }
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
            this.model.Reset(Settings.Default.Rows, Settings.Default.Columns, Settings.Default.Mines);
            this.boardView.Model = this.model;
            this.boardView.Reset(Settings.Default.Rows, Settings.Default.Columns);
            this.dockPanel.IsEnabled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.newGameButton != null)
            {
                this.newGameButton.Click -= newGameButton_Click;
            }
            base.OnClosing(e);
        }

        private void newGameButton_Click(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Reset();
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                            ShowGameOverWindow(false);
                            break;
                        case GameState.Won:
                            ShowGameOverWindow(true);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ShowGameOverWindow(bool win)
        {
            GameEndMessageWindow window = new GameEndMessageWindow(win, new Action(() => Reset()));
            window.Owner = GetWindow(this); // Pop up over the centre of the main window
            window.Show();
            this.dockPanel.IsEnabled = false;
        }

        private void newGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewGameSettingsWindow window = new NewGameSettingsWindow(new Action(() => Reset()));
            window.Owner = GetWindow(this);
            window.Show();
        }
    }
}
