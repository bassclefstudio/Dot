using BassClefStudio.Dot.Core.Levels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    public class Camera
    {
        public Vector2 CameraPosition { get; set; }
        public float Scale { get; set; }

        public Camera()
        {
            CameraPosition = new Vector2(0, 0);
            Scale = 1;
        }

        public Vector2 ProjectVector(Vector2 point)
        {
            return Scale * (point - CameraPosition);
        }

        public void MoveCamera(GameState gameState, float deltaFrames)
        {

        }
    }
}
