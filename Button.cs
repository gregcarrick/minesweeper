using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

namespace Minesweeper
{
    public class Button : FrameworkElement
    {
        private DrawingVisual backgroundVisual;
        private DrawingVisual buttonVisual;
        private DrawingVisual textVisual;
        private DrawingVisual hoverVisual;
        private List<Visual> visuals = new List<Visual>();

        public event MouseEventHandler Click;

        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                DrawText();
            }
        }

        public Button()
        {
            this.backgroundVisual = new DrawingVisual();
            AddDrawingVisual(this.backgroundVisual);

            this.buttonVisual = new DrawingVisual();
            AddDrawingVisual(this.buttonVisual);

            this.textVisual = new DrawingVisual();
            AddDrawingVisual(this.textVisual);

            this.hoverVisual = new DrawingVisual();
            AddDrawingVisual(this.hoverVisual);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Draw();
        }

        private void Draw()
        {
            DrawBackGround();
            DrawButton();
            DrawText();
        }

        private void DrawBackGround()
        {
            using var bv = this.backgroundVisual.RenderOpen();
            bv.DrawRectangle(StaticResources.BgBrush, null, this.GetRect());
            Point origin = new Point(0, 0);
            bv.DrawLines(
                new Pen(StaticResources.ShadowBrush, 1),
                new Point[]
                {
                    origin,
                    new Point(this.Width - 1, 0),
                    new Point(this.Width - 1, this.Height - 1),
                    new Point(0, this.Height - 1),
                    origin
                });
        }

        private void DrawButton()
        {
            using var bv = this.buttonVisual.RenderOpen();
            Pen highlightPen = new Pen(StaticResources.HighlightBrush, 1);
            Pen shadowPen = new Pen(StaticResources.ShadowBrush, 1);
            // Bg
            bv.DrawRectangle(StaticResources.BgBrush, null, this.GetRect());
            ;
            // Highlights
            bv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(1, 0),
                    new Point(this.Width - 1, 0),
                    new Point(this.Width - 1, this.Height - 2)
                });
            bv.DrawLines(
                highlightPen,
                new Point[]
                {
                    new Point(2, 1),
                    new Point(this.Width - 2, 1),
                    new Point(this.Width - 2, this.Height - 3)
                });
            // Shadows
            bv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(0, 1),
                    new Point(0, this.Height - 1),
                    new Point(this.Width - 2, this.Height - 1)
                });
            bv.DrawLines(
                shadowPen,
                new Point[]
                {
                    new Point(1, 2),
                    new Point(1, this.Height - 2),
                    new Point(this.Width - 3, this.Height - 2)
                });
        }

        private void DrawText()
        {
            using var tv = this.textVisual.RenderOpen();
            FormattedText text = new FormattedText(
                this.Text ?? "",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Consolas"),
                16,
                Brushes.Black,
                1);
            double textWidth = text.Width;
            double textHeight = text.Height;
            tv.DrawText(text, text.GetOriginOfCenteredText(this.Width, this.Height));
        }

        private void AddDrawingVisual(DrawingVisual drawingVisual)
        {
            AddVisualChild(drawingVisual);
            this.visuals.Add(drawingVisual);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        protected override int VisualChildrenCount => this.visuals.Count;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                using var bv = this.buttonVisual.RenderOpen(); // Clear
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            DrawButton(); // Redraw button
            using var hv = this.hoverVisual.RenderOpen(); // Clear previous graphic
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            using (var dc = this.buttonVisual.RenderOpen())
            {
                // Clear
            }
            using (var hv = this.hoverVisual.RenderOpen())
            {
                // Clear
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            DrawButton(); // Redraw button
            using var hv = this.hoverVisual.RenderOpen();
            Brush brush = StaticResources.MouseOverBrush;
            hv.DrawRectangle(brush, null, this.GetRect());
            this.Click.Invoke(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                // Draw hover effect.
                using var hv = this.hoverVisual.RenderOpen();
                Brush brush = StaticResources.MouseOverBrush;
                hv.DrawRectangle(brush, null, this.GetRect());
            }
        }
    }
}
