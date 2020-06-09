using Minesweeper.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper
{
    public class BoardView : FrameworkElement
    {
        private const int cellSize = 24;
        private int rowCount;
        private int columnCount;

        List<Visual> visuals = new List<Visual>();
        private DrawingVisual boardVisual;
        private DrawingVisual hoverVisual;

        private Location last;
        private bool isChording;

        public static readonly DependencyProperty BackgroundProperty;
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register("Highlight", typeof(Brush), typeof(BoardView),
                new FrameworkPropertyMetadata(Brushes.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Highlight
        {
            get
            {
                return (Brush)GetValue(HighlightProperty);
            }
            set
            {
                SetValue(HighlightProperty, value);
            }
        }

        public static readonly DependencyProperty ShadowProperty =
            DependencyProperty.Register("Shadow", typeof(Brush), typeof(BoardView),
                new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Shadow
        {
            get
            {
                return (Brush)GetValue(ShadowProperty);
            }
            set
            {
                SetValue(ShadowProperty, value);
            }
        }

        static BoardView()
        {
            // Override the metadata from the Control class's DependencyProperties.
            BackgroundProperty = Control.BackgroundProperty.AddOwner(
                typeof(BoardView),
                new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.AffectsRender)
                );
        }

        public BoardView()
        {
            this.boardVisual = new DrawingVisual();
            AddDrawingVisual(boardVisual);

            this.hoverVisual = new DrawingVisual();
            AddDrawingVisual(hoverVisual);
        }

        public GameModel GameModel { get; set; }

        protected override int VisualChildrenCount => this.visuals.Count;

        public void Reset()
        {
            this.rowCount = Settings.Default.Rows;
            this.columnCount = Settings.Default.Columns;

            this.Width = this.columnCount * cellSize;
            this.Height = this.rowCount * cellSize;

            if (this.boardVisual != null)
            {
                DrawBoard();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DrawBoard();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Location loc = GetLocationFromPoint(e.GetPosition(this));
            if (IsValidLocation(loc))
            {
                using var hv = this.hoverVisual.RenderOpen();
                switch (GetCellStateFromLocation(loc))
                {
                    case CellState.Default:
                        DrawCellPressingOverlay(hv, loc);
                        break;
                    case CellState.Opened:
                        if (e.RightButton == MouseButtonState.Pressed)
                        {
                            // Pressing both buttons together - chording.
                            this.isChording = true;
                        }
                        if (this.isChording)
                        {
                            DrawChordingOverlay(hv, loc);
                        }
                        break;
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Location loc = GetLocationFromPoint(e.GetPosition(this));
            if (IsValidLocation(loc))
            {
                this.GameModel.OpenCell(loc.X, loc.Y, this.isChording);
            }
            this.isChording = false;

            using var hv = this.hoverVisual.RenderOpen();
            if (IsValidLocation(loc) && GetCellStateFromLocation(loc) == CellState.Default)
            {
                DrawCellHoverOverlay(hv, loc);
            }
            else
            {
                // Clear the top layer of any previous graphic.
            }

            DrawBoard();
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Pressing both buttons together - chording
                this.isChording = true;
            }

            Location loc = GetLocationFromPoint(e.GetPosition(this));
            if (!this.isChording && IsValidLocation(loc))
            {
                this.GameModel.FlagCell(loc.X, loc.Y);
            }

            using var hv = this.hoverVisual.RenderOpen();
            if (IsValidLocation(loc))
            {
                switch (GetCellStateFromLocation(loc))
                {
                    case CellState.Default:
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            DrawCellPressingOverlay(hv, loc);
                        }
                        else
                        {
                            DrawCellHoverOverlay(hv, loc);
                        }
                        break;
                    case CellState.Opened:
                        if (this.isChording)
                        {
                            DrawChordingOverlay(hv, loc);
                        }
                        break;
                }
            }
            else
            {
                // Clear the top layer of any previous graphic.
            }

            DrawBoard();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Location loc = GetLocationFromPoint(e.GetPosition(this));

            if (this.last != loc)
            {
                if (this.isChording && e.MiddleButton == MouseButtonState.Released
                    && (e.LeftButton == MouseButtonState.Released || e.RightButton == MouseButtonState.Released))
                {
                    // Was chording, but mouse is now over a different cell and one of the buttons has been released.
                    this.isChording = false;
                }

                using var hv = this.hoverVisual.RenderOpen();
                if (IsValidLocation(loc))
                {
                    switch (GetCellStateFromLocation(loc))
                    {
                        case CellState.Default:
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                DrawCellPressingOverlay(hv, loc);
                            }
                            else
                            {
                                // Draw hover effect.
                                DrawCellHoverOverlay(hv, loc);
                            }
                            break;
                        case CellState.Opened:
                            if (this.isChording)
                            {
                                DrawChordingOverlay(hv, loc);
                            }
                            break;
                        default:
                            // Clear the top layer of any previous graphic.
                            break;
                    }

                    this.last = loc;
                }
                else
                {
                    // Clear the top layer of any previous graphic.
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            // Clear the top layer of any previous graphic.
            using var hv = this.hoverVisual.RenderOpen();
        }


        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        private void AddDrawingVisual(DrawingVisual drawingVisual)
        {
            AddVisualChild(drawingVisual);
            this.visuals.Add(drawingVisual);
        }

        private void DrawBoard()
        {
            if (this.GameModel != null)
            {
                using var bv = this.boardVisual.RenderOpen();
                bv.DrawRectangle(this.Background, null, this.GetRect());

                for (int y = 0; y < this.rowCount; y++)
                {
                    for (int x = 0; x < this.columnCount; x++)
                    {
                        Cell cell = this.GameModel[x, y];
                        switch (cell.State)
                        {
                            case CellState.Detonated:
                                Rect rect = GetCellRectFromCell(cell);
                                // Draw outline
                                bv.DrawRectangle(null, new Pen(this.Shadow, 1), rect);
                                bv.DrawImage(StaticResources.DetonatedMineImage, rect);
                                break;
                            case CellState.Flagged:
                                bv.DrawImage(
                                    (this.GameModel.State == GameState.Lost && !cell.IsMine) ? StaticResources.IncorrectFlagImage : StaticResources.FlagImage,
                                    GetCellRectFromCell(cell));
                                DrawButtonShadows(bv, x, y);
                                break;
                            case CellState.Opened:
                                FormattedText text = this.GetNumberText(cell);
                                if (text != null)
                                {
                                    Point origin = (Point)(
                                        (Vector)text.GetOriginOfCenteredText(cellSize, cellSize)
                                        + new Vector(x * cellSize, y * cellSize));
                                    bv.DrawText(text, origin);
                                }
                                // Draw outline
                                bv.DrawRectangle(null, new Pen(this.Shadow, 1), GetCellRectFromCell(cell));
                                break;
                            case CellState.Default:
                                if (this.GameModel.State == GameState.Lost && cell.IsMine)
                                {
                                    bv.DrawImage(StaticResources.MineImage, GetCellRectFromCell(cell));
                                }
                                else
                                {
                                    DrawButtonShadows(bv, x, y);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private FormattedText GetNumberText(Cell cell)
        {
            Brush brush;
            switch (cell.AdjacentMines)
            {
                case 1:
                    brush = Brushes.Blue;
                    break;
                case 2:
                    brush = Brushes.DarkGreen;
                    break;
                case 3:
                    brush = Brushes.Red;
                    break;
                case 4:
                    brush = Brushes.DarkBlue;
                    break;
                case 5:
                    brush = Brushes.DarkRed;
                    break;
                case 6:
                    brush = Brushes.Teal;
                    break;
                case 7:
                    brush = Brushes.DarkViolet;
                    break;
                case 8:
                    brush = Brushes.DimGray;
                    break;
                default:
                    // No text if there are 0 mines.
                    return null;
            }

            return new FormattedText(
                cell.AdjacentMines.ToString(),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                16,
                brush,
                1);
        }

        private void DrawButtonShadows(DrawingContext dc, int x, int y)
        {
            // Draw shadows along the left and bottom sides.
            dc.DrawLines(
                new Pen(this.Shadow, 1),
                new Point[]
                {
                    new Point(x * cellSize, y * cellSize + 1),
                    new Point(x * cellSize, (y + 1) * cellSize - 1),
                    new Point((x + 1) * cellSize - 2, (y + 1) * cellSize - 1)
                });
            dc.DrawLines(
                new Pen(this.Shadow, 1),
                new Point[]
                {
                    new Point(x * cellSize + 1, y * cellSize + 2),
                    new Point(x * cellSize + 1, (y + 1) * cellSize - 2),
                    new Point((x + 1) * cellSize - 3, (y + 1) * cellSize - 2)
                });
            // Draw highlights along the top and right sides.
            dc.DrawLines(
                new Pen(this.Highlight, 1),
                new Point[]
                {
                    new Point(x * cellSize + 1, y * cellSize),
                    new Point((x + 1) * cellSize - 1, y * cellSize),
                    new Point((x + 1) * cellSize - 1, (y + 1) * cellSize - 2)
                });
            dc.DrawLines(
                new Pen(this.Highlight, 1),
                new Point[]
                {
                    new Point(x * cellSize + 2, y * cellSize + 1),
                    new Point((x + 1) * cellSize - 2, y * cellSize + 1),
                    new Point((x + 1) * cellSize - 2, (y + 1) * cellSize - 3)
                });
        }

        private bool IsValidLocation(Location location)
        {
            return location.X >= 0 && location.X < this.columnCount &&
                location.Y >= 0 && location.Y < this.rowCount;
        }

        private static Location GetLocationFromPoint(Point pt)
        {
            // Round towards zero.
            return new Location((int)(pt.X / cellSize), (int)(pt.Y / cellSize));
        }

        private static Rect GetCellRectFromCell(Cell cell)
        {
            return new Rect(cell.X * cellSize, cell.Y * cellSize, cellSize, cellSize);
        }

        private static Rect GetCellRectFromLocation(Location loc)
        {
            return new Rect(loc.X * cellSize, loc.Y * cellSize, cellSize, cellSize);
        }

        private CellState GetCellStateFromLocation(Location loc)
        {
            return this.GameModel[loc.X, loc.Y].State;
        }

        private void DrawCellHoverOverlay(DrawingContext dc, Location loc)
        {
            dc.DrawRectangle(StaticResources.MouseOverBrush, null, GetCellRectFromLocation(loc));
        }

        private void DrawCellPressingOverlay(DrawingContext dc, Location loc)
        {
            dc.DrawRectangle(this.Background, new Pen(this.Shadow, 1), GetCellRectFromLocation(loc));
        }

        private void DrawChordingOverlay(DrawingContext dc, Location loc)
        {
            for (int x = Math.Max(0, loc.X - 1); x <= Math.Min(this.columnCount - 1, loc.X + 1); x++)
            {
                for (int y = Math.Max(0, loc.Y - 1); y <= Math.Min(this.rowCount - 1, loc.Y + 1); y++)
                {
                    if (this.GameModel[x, y].State == CellState.Default)
                    {
                        DrawCellPressingOverlay(dc, new Location(x, y));
                    }
                }
            }
        }
    }
}
