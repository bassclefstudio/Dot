using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core
{
    public class GameState
    {
        public Map Map { get; set; }

        public Player Player { get; set; }

        public void DoPhysics()
        {
            Player.DoPhysics(Map.CurrentLevel.Segments);
        }
    }
}
