using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Map
    {
        public List<Level> Levels { get; }

        private int levelIndex;
        public int LevelIndex { get => levelIndex; set { levelIndex = value; CurrentLevelChanged?.Invoke(this, new EventArgs()); } }
        public Level CurrentLevel => Levels.ElementAtOrDefault(LevelIndex);

        public event EventHandler CurrentLevelChanged;

        public Map()
        {
            Levels = new List<Level>();
        }

        public Map(IEnumerable<Level> levels)
        {
            Levels = new List<Level>(levels);
        }

        public void NextLevel()
        {
            LevelIndex = (LevelIndex + 1) % Levels.Count;
        }

        public void SetLevel(Level level)
        {
            if(level != null && Levels.Contains(level))
            {
                LevelIndex = Levels.IndexOf(level);
            }
        }
    }
}
