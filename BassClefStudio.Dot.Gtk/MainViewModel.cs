using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Core.Rendering;
using BassClefStudio.Dot.Serialization;
using BassClefStudio.NET.Core;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Dot.Gtk
{
    public class MainViewModel : Observable
    {
        public GameState GameState { get; set; }

        public GameRenderer GameRenderer { get; private set; }

        public MainViewModel(GameState gameState, GameRenderer renderer)
        {
            GameState = gameState;
            GameRenderer = renderer;

            GameRenderer.GameState = GameState;
            GameRenderer.ViewCamera = GameState.Camera;
            SynchronousTask initTask = new SynchronousTask(PlayDefault);
            initTask.RunTask();
        }

        public void PaintSurface(SKCanvas canvas, SKSize canvasSize)
        {
            GameState.Camera.SetView(new Vector2(canvasSize.Width, canvasSize.Height), 2);
            GameRenderer.Render(canvas);
        }

        public void Update(float deltaFrames)
        {
            GameState.Update(deltaFrames);
        }

        public async Task PlayDefault()
        {
            using (JsonGameService serializerService = new JsonGameService())
            {
                var json = await Task.Run(() => File.ReadAllText(Path.Combine("Data", "Game.json")));
                GameState.Map = serializerService.ReadItem(JToken.Parse(json));
            }
        }
    }
}
