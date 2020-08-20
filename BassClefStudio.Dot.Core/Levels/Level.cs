using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Level
    {
        public string Name { get; }
        public IEnumerable<Segment> Segments { get; }

        public Level(string name, IEnumerable<Segment> segments)
        {
            Name = name;
            Segments = segments;
        }
    }
}
