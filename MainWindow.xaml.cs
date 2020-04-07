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
            this.model.Reset(this.RowCount, this.ColumnCount, this.Mines);
            this.boardView.Model = this.model;
            this.boardView.Reset(this.RowCount, this.ColumnCount);
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
                    GameEndMessageWindow window;
                    switch (this.model.State)
                    {
                        case GameState.Lost:
                            window = new GameEndMessageWindow(false, this);
                            window.Owner = Window.GetWindow(this); // Opens the window over the game window.
                            window.Show();
                            break;
                        case GameState.Won:
                            window = new GameEndMessageWindow(true, this);
                            window.Owner = Window.GetWindow(this);
                            window.Show();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
