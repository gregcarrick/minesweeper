using System;
using System.Windows.Forms;

namespace Minesweeper
{
    public class MinesweeperButton : Button
    {
        public event EventHandler<ButtonStateEventArgs> StateChanged;

        public MinesweeperButton(int x, int y)
            : base()
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public ButtonState State { get; private set; }

        public void SetMinesNumber(int adjacentMines)
        {
            this.Text = adjacentMines.ToString();
        }

        public void Open()
        {
            ChangeState(ButtonState.Opened);
        }

        public void Detonate()
        {
            this.BackColor = System.Drawing.Color.Black;
            ChangeState(ButtonState.Detonated);
        }

        public void Freeze()
        {
            this.State = ButtonState.Frozen;
        }

        public void Flag()
        {
            this.BackColor = System.Drawing.Color.Red;
            ChangeState(ButtonState.Flagged);
        }

        private void Unflag()
        {
            this.BackColor = DefaultBackColor;
            ChangeState(ButtonState.Default);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.State != ButtonState.Frozen)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this.State == ButtonState.Default)
                    {
                        Open();
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    switch (this.State)
                    {
                        case ButtonState.Default:
                            ChangeState(ButtonState.Flagged);
                            break;
                        case ButtonState.Flagged:
                            ChangeState(ButtonState.Default);
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void ChangeState(ButtonState newState)
        {
            ButtonStateEventArgs e = new ButtonStateEventArgs(this.State, newState);
            this.State = newState;

            switch(newState)
            {
                case ButtonState.Default:
                    this.BackColor = Control.DefaultBackColor;
                    OnStateChanged(e);
                    break;
                case ButtonState.Flagged:
                    this.BackColor = System.Drawing.Color.Red;
                    OnStateChanged(e);
                    break;
                case ButtonState.Opened:
                    this.BackColor = System.Drawing.Color.White;
                    OnStateChanged(e);
                    break;
                case ButtonState.Detonated:
                    this.BackColor = System.Drawing.Color.Black;
                    break;
                case ButtonState.Frozen:
                    break;
            }
        }

        private void OnStateChanged(ButtonStateEventArgs e)
        {
            this.StateChanged?.Invoke(this, e);
        }
    }
}
