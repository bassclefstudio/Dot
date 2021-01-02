using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Core.Levels
{
    public class Map
    {
        /// <summary>
        /// A value indicating the current schema/feature version of the <see cref="Map"/> class.
        /// </summary>
        public const int Version = 5;

        /// <summary>
        /// A list of the available <see cref="Level"/>s on this <see cref="Map"/>.
        /// </summary>
        public List<Level> Levels { get; }

        private int levelIndex;
        /// <summary>
        /// The <see cref="int"/> index (in <see cref="Levels"/>) of the <see cref="CurrentLevel"/>.
        /// </summary>
        public int LevelIndex { get => levelIndex; set { levelIndex = value; CurrentLevelChanged?.Invoke(this, new EventArgs()); } }
        /// <summary>
        /// The <see cref="Level"/> (from the <see cref="Levels"/> collection) that is currently active.
        /// </summary>
        public Level CurrentLevel => Levels.ElementAtOrDefault(LevelIndex);

        /// <summary>
        /// An event fired when the <see cref="CurrentLevel"/>/<see cref="LevelIndex"/> changes.
        /// </summary>
        public event EventHandler CurrentLevelChanged;

        /// <summary>
        /// Creates a new blank <see cref="Map"/>.
        /// </summary>
        public Map()
        {
            Levels = new List<Level>();
        }

        /// <summary>
        /// Creates a populated <see cref="Map"/>.
        /// </summary>
        /// <param name="levels">A collection of the available <see cref="Level"/>s on this <see cref="Map"/>.</param>
        public Map(IEnumerable<Level> levels)
        {
            Levels = new List<Level>(levels);
        }

        /// <summary>
        /// Iterates to the next current level of the <see cref="Levels"/> in the game.
        /// </summary>
        public void NextLevel()
        {
            LevelIndex = (LevelIndex + 1) % Levels.Count;
        }

        /// <summary>
        /// Sets the <see cref="CurrentLevel"/> to the provided <see cref="Level"/>.
        /// </summary>
        /// <param name="level">The desired new <see cref="CurrentLevel"/>.</param>
        public void SetLevel(Level level)
        {
            if(level != null && Levels.Contains(level))
            {
                LevelIndex = Levels.IndexOf(level);
            }
        }
    }
}
