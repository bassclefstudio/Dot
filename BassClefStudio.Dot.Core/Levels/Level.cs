using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Level
    {
        public IEnumerable<Segment> Segments { get; }

        public Level(IEnumerable<Segment> segments)
        {
            Segments = segments;
        }
    }
}
