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
using Decklan.NET.Core;
using Decklan.UWP.Core;
using Decklan.UWP.Navigation.DI;
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
        }

        public async Task Initialize()
        { }

        public void PaintSurface(SKCanvas canvas, SKSize canvasSize)
        {
            GameState.Camera.SetView(new Vector2(canvasSize.Width, canvasSize.Height), 4);
            GameRenderer.Render(GameState, canvas);
        }

        public void Update(float deltaFrames)
        {
            GameState.Update(deltaFrames);
        }
    }
}
