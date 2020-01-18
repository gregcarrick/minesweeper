using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class CounterControl : UserControl
    {
        public CounterControl()
        {
            InitializeComponent();

            this.Paint += CounterControl_Paint;
        }

        public int Value
        {
            get
            {
                return Convert.ToInt32(this.label?.Text, CultureInfo.InvariantCulture);
            }
            set
            {
                if (this.label != null)
                {
                    if (value > -100 && value < 0)
                    {
                        this.label.Text = value.ToString("d2", CultureInfo.InvariantCulture);
                    }
                    else if (value >= 0 && value < 1000)
                    {
                        this.label.Text = value.ToString("d3", CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        /// <summary>
        /// Add shading around frame
        /// </summary>
        private void CounterControl_Paint(object sender, PaintEventArgs e)
        {
            // Shade outside bottom and right
            e.Graphics.DrawLines(
                Pens.Gray,
                new Point[]
                {
                    new Point(1,                this.Height - 1 ),
                    new Point(this.Width - 1,   this.Height - 1 ),
                    new Point(this.Width - 1,   1               ),
                });
            // Shade inside top and left
            e.Graphics.DrawLines(
                Pens.Gray,
                new Point[]
                {
                    new Point(2,                this.Height - 4 ),
                    new Point(2,                2               ),
                    new Point(this.Width - 4,   2               ),
                });
            // Highlight outside top and left
            e.Graphics.DrawLines(
                Pens.WhiteSmoke,
                new Point[]
                {
                    new Point(0,                this.Height - 2 ),
                    new Point(0,                0               ),
                    new Point(this.Width - 2,   0               ),
                });
            // Hightlight inside bottom and right
            e.Graphics.DrawLines(
                Pens.WhiteSmoke,
                new Point[]
                {
                    new Point(3,                this.Height - 3 ),
                    new Point(this.Width - 3,   this.Height - 3 ),
                    new Point(this.Width - 3,                 3 ),
                });
        }
    }
}
