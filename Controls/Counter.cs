using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Minesweeper
{
    public class Counter : FrameworkElement
    {
        private DrawingVisual drawingVisual;
        private DrawingVisual textVisual;
        private List<Visual> visuals = new List<Visual>();

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(Counter),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);

                DrawText();
            }
        }

        public Counter()
        {
            this.Width = 57;
            this.Height = 34;

            this.drawingVisual = new DrawingVisual();
            AddLogicalChild(this.drawingVisual);
            AddVisualChild(this.drawingVisual);
            this.visuals.Add(this.drawingVisual);

            this.textVisual = new DrawingVisual();
            AddLogicalChild(this.textVisual);
            AddVisualChild(this.textVisual);
            this.visuals.Add(this.textVisual);
        }

        protected override int VisualChildrenCount => this.visuals.Count;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DrawBorder();
            DrawText();
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        private void DrawBorder()
        {
            using var dv = this.drawingVisual.RenderOpen();
            dv.DrawRectangle(StaticResources.BgBrush, null, new Rect(0, 0, this.Width, this.Height));
            dv.DrawRectangle(Brushes.Black, null, new Rect(6, 6, this.Width - 12, this.Height - 12));

            // Draw shadows
            Pen shadowPen = new Pen(StaticResources.ShadowBrush, 1);
            dv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(0, 1),
                    new Point(0, this.Height - 1),
                    new Point(this.Width - 2, this.Height - 1)
                });
            dv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(1, 2),
                    new Point(1, this.Height - 2),
                    new Point(this.Width - 3, this.Height - 2)
                });
            dv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(5, 4),
                    new Point(this.Width - 5, 4),
                    new Point(this.Width - 5, this.Height - 6)
                });
            dv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(6, 5),
                    new Point(this.Width - 6, 5),
                    new Point(this.Width - 6, this.Height - 7)
                });
            // Draw highlights
            Pen highlightPen = new Pen(StaticResources.HighlightBrush, 1);
            dv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(1, 0),
                    new Point(this.Width - 1, 0),
                    new Point(this.Width - 1, this.Height - 2)
                });
            dv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(2, 1),
                    new Point(this.Width - 2, 1),
                    new Point(this.Width - 2, this.Height - 3)
                });
            dv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(4, 5),
                    new Point(4, this.Height - 5),
                    new Point(this.Width - 6, this.Height - 5)
                });
            dv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(5, 6),
                    new Point(5, this.Height - 6),
                    new Point(this.Width - 7, this.Height - 6)
                });
        }

        private void DrawText()
        {
            using var tv = this.textVisual.RenderOpen();
            // This is a 3-digit display.
            int val = Math.Max(-99, Math.Min(999, this.Value));
            string text;
            if (val < 0)
            {
                text = "-" + Math.Abs(val).ToString().PadLeft(2, '0');
            }
            else
            {
                text = val.ToString().PadLeft(3, '0');
            }
            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Consolas"),
                22,
                Brushes.Red,
                1);
            tv.DrawText(formattedText, formattedText.GetOriginOfCenteredText(this.Width, this.Height));
        }
    }
}
