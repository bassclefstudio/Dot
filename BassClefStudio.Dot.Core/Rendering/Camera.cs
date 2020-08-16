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

        public Vector2 ProjectVector(Vector2 point)
        {
            return Scale * (point - CameraPosition);
        }
    }
}
