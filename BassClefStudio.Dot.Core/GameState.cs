using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using BassClefStudio.Dot.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core
{
    public class GameState
    {
        private Map map;
        public Map Map { get => map; set { MapChanged(map, value); map = value; ResetLevel(); } }

        public Player Player { get; }

        public Camera Camera { get; }

        public Inputs Inputs { get; }

        public GameState()
        {
            Player = new Player();
            Camera = new Camera();
            Inputs = new Inputs();
        }

        private void MapChanged(Map oldMap, Map newMap)
        {
            if (oldMap != null)
            {
                oldMap.CurrentLevelChanged -= CurrentLevelChanged;
            }

            if (newMap != null)
            {
                newMap.CurrentLevelChanged += CurrentLevelChanged;
            }
        }

        private void CurrentLevelChanged(object sender, EventArgs e)
            => ResetLevel();
        public void ResetLevel()
        {
            Player.SetStartingPos(Map.CurrentLevel);
            // Reset camera instantly.
            Camera.MoveCamera(this);
        }

        public void Update(float deltaFrames)
        {
            if (Map != null && Map.CurrentLevel != null)
            {
                Player.DoPhysics(this, deltaFrames);
                Camera.MoveCamera(this, deltaFrames, 10);
            }
        }
    }
}
