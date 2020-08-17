using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    public class GameRenderer : IDisposable
    {
        private Dictionary<string, SKPaint> Paints = new Dictionary<string, SKPaint>();

        private const int GameSizeX = 480;
        private const int GameSizeY = 360;

        public GameRenderer()
        {
            Paints.Add("Line", new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
            });

            var wallPaint = Paints["Line"].Clone();
            wallPaint.Color = new SKColor(255, 255, 255);
            wallPaint.StrokeWidth = 8;
            Paints.Add("Wall", wallPaint);

            var playerPaint = Paints["Line"].Clone();
            playerPaint.Color = new SKColor(255, 255, 255);
            playerPaint.StrokeWidth = 12;
            Paints.Add("Player", playerPaint);

            var borderPaint = Paints["Line"].Clone();
            borderPaint.Color = new SKColor(255, 255, 255);
            borderPaint.StrokeWidth = 4;
            Paints.Add("Border", borderPaint);
        }

        public void Render(GameState gameState, SKCanvas canvas, SKSize canvasSize)
        {
            canvas.Clear();
            canvas.Scale(canvasSize.Width / GameSizeX, -canvasSize.Height / GameSizeY);
            canvas.Translate(GameSizeX / 2f, -GameSizeY / 2f);
            SKRect boundingRect = new SKRect(-GameSizeX / 2f, GameSizeY / 2f, GameSizeX / 2, -GameSizeY / 2);
            canvas.ClipRect(boundingRect, SKClipOperation.Intersect, true);
            canvas.DrawRect(boundingRect, Paints["Border"]);

            if (gameState.Map.CurrentLevel != null)
            {
                foreach (var segment in gameState.Map.CurrentLevel.Segments)
                {
                    if (segment.Type == SegmentType.Wall)
                    {
                        Vector2 p1 = gameState.Camera.ProjectVector(segment.Point1);
                        Vector2 p2 = gameState.Camera.ProjectVector(segment.Point2.Value);
                        canvas.DrawLine(
                            p1.X,
                            p1.Y,
                            p2.X,
                            p2.Y,
                            Paints["Wall"]);
                    }
                }

                Debug.WriteLine("Rendering player...");
                byte alpha = 0;
                for (int i = 0; i < gameState.Player.Ghosts.Count; i++)
                {
                    alpha += (byte)(255 / gameState.Player.Ghosts.Count);
                    SKPaint ghostPaint = Paints["Player"].Clone();
                    ghostPaint.StrokeWidth = i * (12 / gameState.Player.Ghosts.Count);
                    ghostPaint.Color = new SKColor(255, 255, 255, alpha);

                    canvas.DrawPoint(gameState.Player.Ghosts[i].X, gameState.Player.Ghosts[i].Y, ghostPaint);
                }

                canvas.DrawPoint(gameState.Player.Position.X, gameState.Player.Position.Y, Paints["Player"]);
            }
        }

        public void Dispose()
        {
            foreach (var paint in Paints)
            {
                paint.Value.Dispose();
            }
        }
    }
}
