using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    public class GameRenderer : IRenderer<GameState>
    {
        public IEnumerable<IRenderer<Segment>> SegmentRenderers { get; }

        public IRenderer<Player> PlayerRenderer { get; }

        public GameRenderer(IEnumerable<IRenderer<Segment>> segments, IRenderer<Player> player)
        {
            SegmentRenderers = segments;
            PlayerRenderer = player;
        }

        public bool CanRender(GameState value) => value != null;

        public void Render(GameState gameState, SKCanvas canvas)
        {
            foreach (var segment in gameState.Map.CurrentLevel.Segments)
            {
                SegmentRenderers.FirstOrDefault(r => r.CanRender(segment))?.Render(segment, canvas);
            }

            if (PlayerRenderer.CanRender(gameState.Player))
            {
                PlayerRenderer.Render(gameState.Player, canvas);
            }
        }
    }
}
