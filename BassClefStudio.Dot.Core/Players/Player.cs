using BassClefStudio.Dot.Core.Levels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Core.Physics
{
    public class Player
    {
        public Vector2 Position { get; set; }

        public IEnumerable<IPhysicsHandler> PhysicsHandlers { get; }

        public Player(IEnumerable<IPhysicsHandler> physicsHandlers)
        {
            PhysicsHandlers = physicsHandlers;
        }

        public void DoPhysics(IEnumerable<Segment> segments)
        {
            foreach (var handler in PhysicsHandlers)
            {
                handler.DoPhysics(this, segments);
            }
        }
    }
}
