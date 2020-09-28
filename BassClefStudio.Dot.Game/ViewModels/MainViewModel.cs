using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Rendering;
using BassClefStudio.Dot.Serialization;
using BassClefStudio.NET.Core;
using BassClefStudio.UWP.Core;
using BassClefStudio.UWP.Navigation.DI;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Windows.Storage;
using Windows.Storage.Pickers;

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
            GameRenderer.AttachedContext = GameState;
            GameRenderer.ViewCamera = GameState.Camera;
        }

        public async Task Initialize()
        { }

        public void PaintSurface(SKCanvas canvas, SKSize canvasSize)
        {
            GameState.Camera.SetView(new Vector2(canvasSize.Width, canvasSize.Height), 4);
            GameRenderer.Render(canvas);
        }

        public void Update(float deltaFrames)
        {
            GameState.Update(deltaFrames);
        }
    }
}
