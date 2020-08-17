using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Rendering;
using Decklan.NET.Core;
using Decklan.UWP.Navigation.DI;
using SkiaSharp;

namespace BassClefStudio.Dot.Game.ViewModels
{
    public class MainViewModel : Observable, IViewModel
    {
        public GameState GameState { get; set; }

        public GameRenderer GameRenderer { get; private set; }

        public MainViewModel(GameState gameState, GameRenderer renderer)
        {
            GameState = gameState;
            GameRenderer = renderer;
        }

        public async Task Initialize()
        {
            GameState.Map.Levels.Add(
                new Level(
                    new Segment[] 
                    {
                        new Segment(SegmentType.Wall, new Vector2(0,-60), new Vector2(100,-60)),
                        new Segment(SegmentType.Wall, new Vector2(0,60), new Vector2(180,60))
                    }));
        }

        public void PaintSurface(SKCanvas canvas, SKSize canvasSize)
        {
            GameRenderer.Render(GameState, canvas, canvasSize);
        }

        public void Update(float deltaFrames)
        {
            GameState.Update(deltaFrames);
        }
    }
}
