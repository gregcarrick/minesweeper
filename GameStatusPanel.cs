using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class GameStatusPanel : UserControl
    {
        public GameStatusPanel()
        {
            InitializeComponent();

            this.Paint += GameStatusPanel_Paint;
        }

        /// <summary>
        /// Add shading around frame
        /// </summary>
        private void GameStatusPanel_Paint(object sender, PaintEventArgs e)
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
