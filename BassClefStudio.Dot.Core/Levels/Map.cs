using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Map
    {
        public IEnumerable<Level> Levels { get; }
        public int LevelIndex { get; set; }
        public Level CurrentLevel => Levels.ElementAtOrDefault(LevelIndex);

        public Map(IEnumerable<Level> levels)
        {
            Levels = levels;
        }
    }
}
