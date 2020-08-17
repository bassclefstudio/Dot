using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Physics
{
    public class Inputs
    {
        public bool Jump { get; set; }

        private float move;
        public float Move { get => move; set { move = value; SetMoveDirs(); } }

        private bool moveRight;
        public bool MoveRight { get => moveRight; set { moveRight = value; SetMove(); } }

        private bool moveLeft;
        public bool MoveLeft { get => moveLeft; set { moveLeft = value; SetMove(); } }

        private void SetMove()
        {
            move = (MoveRight ? 1 : 0) + (MoveLeft ? -1 : 0);
        }

        private void SetMoveDirs()
        {
            moveRight = Move > 0;
            moveLeft = Move < 0;
        }
    }
}
