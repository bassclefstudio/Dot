using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using BassClefStudio.Dot.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core
{
    public class GameState
    {
        public Map Map { get; set; }

        public Player Player { get; }

        public Camera Camera { get; }

        public Inputs Inputs { get; }

        public GameState()
        {
            Map = new Map();
            Camera = new Camera();
            Player = new Player();
            Inputs = new Inputs();
        }

        public void Update(float deltaFrames)
        {
            Player.DoPhysics(this, deltaFrames);
            Camera.MoveCamera(this, deltaFrames);
        }
    }
}
