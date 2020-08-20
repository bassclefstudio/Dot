using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Segment
    {
        public SegmentType Type { get; }
        public Vector2 Point1 { get; set; }
        public Vector2? Point2 { get; set; }

        public string Id { get; set; }
        public float? Arg { get; set; }

        public Segment(SegmentType type, Vector2 point1, string id = null, float? arg = null)
        {
            Type = type;
            Point1 = point1;
            Id = id;
            Arg = arg;
        }

        public Segment(SegmentType type, Vector2 point1, Vector2 point2, string id = null, float? arg = null)
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
        UI = 5,
        Camera = 6,
        Start = 7,
        End = 8
    }
}
