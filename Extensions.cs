using System.Windows;
using System.Windows.Media;

namespace Minesweeper
{
    public static class Extensions
    {
        /// <summary>
        /// Extends <see cref="DrawingContext.DrawLine(Pen, Point, Point)"/> to
        /// allow drawing multiple line segments connected end-to-end in a chain.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="pen"></param>
        /// <param name="origin">The initial point at one end of the chain of line segments</param>
        /// <param name="points">The set up subsequent points in the chain.</param>
        public static void DrawLines(this DrawingContext dc, Pen pen, Point[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                dc.DrawLine(pen, points[i - 1], points[i]);
            }
        }

        /// <summary>
        /// Returns a rectangle corresponding to the area of a <see cref="FrameworkElement"/>.
        /// </summary>
        public static Rect GetRect(this FrameworkElement fe)
        {
            return new Rect(0, 0, fe.Width, fe.Height);
        }

        /// <summary>
        /// Gets the origin point necessary to centre a formatted text in a containing rectangle.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="containerWidth">The width of the containing rectangle</param>
        /// <param name="containerHeight">The height of the continaing rectangle</param>
        public static Point GetOriginOfCenteredText(this FormattedText text, double containerWidth, double containerHeight)
        {
            return new Point((containerWidth - text.Width) / 2, (containerHeight - text.Height) / 2);
        }
    }
}
