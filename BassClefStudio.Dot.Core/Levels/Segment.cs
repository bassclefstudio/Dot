using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Segment
    {
        public SegmentType Type { get; }
        public Vector2 Point1 { get; }
        public Vector2? Point2 { get; }

        public string Id { get; }
        public string Arg1 { get; }
        public float? ArgNum { get; }

        public Segment(SegmentType type, Vector2 point1, string id = null, string arg = null)
        {
            Type = type;
            Point1 = point1;
            Id = id;
            Arg1 = arg;
            ArgNum = float.TryParse(Arg1, out var f) ? f : (float?)null;
        }

        public Segment(SegmentType type, Vector2 point1, Vector2 point2, string id = null, string arg = null)
            : this(type, point1, id, arg)
        {
            Point2 = point2;
        }
    }

    public enum SegmentType
    {
        Wall = 0,
        Lava = 1,
        Bounce = 2,
        Flip = 3,
        Portal = 4,
        Teleport = 5,
        UI = 6,
        Camera = 7,
        Start = 8,
        End = 9
    }
}
