using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Physics
{
    /// <summary>
    /// Represents a management of all game inputs and their values.
    /// </summary>
    public class Inputs
    {
        /// <summary>
        /// Is the jump trigger active?
        /// </summary>
        public bool Jump { get; set; }

        private float move;
        /// <summary>
        /// A float from -1 to 1 indicating the move control's value (keyboard: -1,0,1; touch: variable).
        /// </summary>
        public float Move { get => move; set { move = value; SetMoveDirs(); } }

        private bool moveRight;
        /// <summary>
        /// A <see cref="bool"/> that can be set to trigger the movement of the player to the right (adjusts <see cref="Move"/>).
        /// </summary>
        public bool MoveRight { get => moveRight; set { moveRight = value; SetMove(); } }

        private bool moveLeft;
        /// <summary>
        /// A <see cref="bool"/> that can be set to trigger the movement of the player to the left (adjusts <see cref="Move"/>).
        /// </summary>
        public bool MoveLeft { get => moveLeft; set { moveLeft = value; SetMove(); } }

        /// <summary>
        /// Sets the <see cref="Move"/> value to -1,0, or 1 depending on the configuration of the <see cref="MoveLeft"/> and <see cref="MoveRight"/> <see cref="bool"/>s.
        /// </summary>
        private void SetMove()
        {
            move = (MoveRight ? 1 : 0) + (MoveLeft ? -1 : 0);
        }

        /// <summary>
        /// Sets the <see cref="MoveLeft"/> and <see cref="MoveRight"/> <see cref="bool"/>s from the <see cref="Move"/> value.
        /// </summary>
        private void SetMoveDirs()
        {
            moveRight = Move > 0;
            moveLeft = Move < 0;
        }
    }
}
