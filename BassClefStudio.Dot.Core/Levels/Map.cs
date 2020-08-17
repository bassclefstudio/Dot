using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Map
    {
        public List<Level> Levels { get; }
        public int LevelIndex { get; set; }
        public Level CurrentLevel => Levels.ElementAtOrDefault(LevelIndex);

        public Map()
        {
            Levels = new List<Level>();
        }

        public Map(IEnumerable<Level> levels)
        {
            Levels = new List<Level>(levels);
        }
    }
}
