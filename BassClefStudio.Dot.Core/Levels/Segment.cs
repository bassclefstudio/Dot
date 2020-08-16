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
        public Vector2 Point2 { get; set; }

        public string Id { get; set; }
        public int Arg { get; set; }
    }

    public enum SegmentType
    {
        Wall = 0
    }
}
