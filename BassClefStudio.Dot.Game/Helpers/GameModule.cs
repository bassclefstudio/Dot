using Autofac;
using BassClefStudio.Dot.Core.Physics;
using BassClefStudio.Dot.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core
{
    public class GameModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GameRenderer>();
            builder.RegisterType<GameState>().SingleInstance();
        }
    }
}
