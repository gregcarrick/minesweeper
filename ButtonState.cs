using System;

namespace Minesweeper
{
    public class ButtonStateEventArgs : EventArgs
    {
        public ButtonStateEventArgs(ButtonState oldState, ButtonState newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }

        public ButtonState OldState { get; private set; }

        public ButtonState NewState { get; private set; }
    }

    public enum ButtonState
    {
        Default,
        Flagged,
        Opened,
        Detonated,
        Frozen,
    }
}
