using BassClefStudio.Dot.Core.Levels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Physics
{
    public interface IPhysicsHandler
    {
        void DoPhysics(Player player, IEnumerable<Segment> segments);
    }
}
