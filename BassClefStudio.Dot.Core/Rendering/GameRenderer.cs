using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    public class GameRenderer : IDisposable
    {
        private Dictionary<string, SKPaint> Paints = new Dictionary<string, SKPaint>();

        public GameRenderer()
        {
            Paints.Add("Base", new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
            });

            void AddPaint(string name, string parent, SKColor color, float strokeWidth = 10)
            {
                var paint = Paints[parent].Clone();
                paint.Color = color;
                paint.StrokeWidth = strokeWidth;
                Paints.Add(name, paint);
            }

            AddPaint("Border", "Base", new SKColor(255, 255, 255), 4);
            AddPaint("Player", "Base", new SKColor(255, 255, 255), 12);
            AddPaint("Wall", "Base", new SKColor(255, 255, 255));
            AddPaint("Bounce", "Base", new SKColor(200, 200, 0));
            AddPaint("Lava", "Base", new SKColor(255, 80, 80));
            AddPaint("Portal", "Base", new SKColor(160, 0, 200), 16);
            AddPaint("Teleport", "Base", new SKColor(160, 0, 200), 6);
            AddPaint("Flip", "Base", new SKColor(255, 0, 255), 4);
            AddPaint("UI", "Base", new SKColor(160, 0, 200), 4);
            Paints["UI"].Typeface = SKTypeface.Default;
            AddPaint("End", "Base", new SKColor(100, 255, 100), 16);
        }

        public void Render(GameState gameState, SKCanvas canvas)
        {
            canvas.Clear();
            float viewScale = gameState.Camera.ViewScale;
            Vector2 canvasSize = gameState.Camera.ViewSize;
            canvas.Scale(viewScale, -viewScale);
            float width = canvasSize.X / (2 * viewScale);
            float height = canvasSize.Y / (2 * viewScale);
            canvas.Translate(width, -height);

            SKRect boundingRect = new SKRect(-width, height, width, -height);
            canvas.ClipRect(boundingRect, SKClipOperation.Intersect, true);
            //canvas.DrawRect(boundingRect, Paints["Border"]);

            if (gameState.Map != null)
            {
                canvas.Scale(gameState.Camera.CameraScale);
                canvas.Translate(-gameState.Camera.CameraPosition.X, -gameState.Camera.CameraPosition.Y);

                if (gameState.Map.CurrentLevel != null)
                {
                    void DrawLine(Segment segment, string paintKey)
                    {
                        Vector2 p1 = segment.Point1;
                        Vector2 p2 = segment.Point2.Value;
                        canvas.DrawLine(
                            p1.X,
                            p1.Y,
                            p2.X,
                            p2.Y,
                            Paints[paintKey]);
                    }

                    void DrawPoint(Segment segment, string paintKey)
                    {
                        Vector2 p1 = segment.Point1;
                        canvas.DrawPoint(
                            p1.X,
                            p1.Y,
                            Paints[paintKey]);
                    }

                    void WrapLines(string longLine, Vector2 point1, Vector2 point2, SKPaint paint)
                    {
                        var wrappedLines = new List<string>();
                        var lineLength = 0f;
                        var line = "";

                        float lineLengthLimit = Math.Abs(point2.X - point1.X);

                        foreach (var word in longLine.Split(' '))
                        {
                            var wordWithSpace = word + " ";
                            var wordWithSpaceLength = paint.MeasureText(wordWithSpace);
                            if (lineLength + wordWithSpaceLength > lineLengthLimit)
                            {
                                wrappedLines.Add(line);
                                line = "" + wordWithSpace;
                                lineLength = wordWithSpaceLength;
                            }
                            else
                            {
                                line += wordWithSpace;
                                lineLength += wordWithSpaceLength;
                            }
                        }

                        var x = point1.X;
                        var y = point1.Y;
                        foreach (var wrappedLine in wrappedLines)
                        {
                            canvas.DrawText(wrappedLine, x, y, paint);
                            y += paint.FontSpacing;
                        }
                    }

                    foreach (var segment in gameState.Map.CurrentLevel.Segments)
                    {
                        if (segment.Type == SegmentType.Wall)
                        {
                            DrawLine(segment, "Wall");
                        }
                        else if (segment.Type == SegmentType.Bounce)
                        {
                            DrawLine(segment, "Bounce");
                        }
                        else if (segment.Type == SegmentType.Lava)
                        {
                            DrawLine(segment, "Lava");
                        }
                        else if (segment.Type == SegmentType.Flip)
                        {
                            DrawLine(segment, "Flip");
                        }
                        else if (segment.Type == SegmentType.Portal)
                        {
                            DrawPoint(segment, "Portal");
                        }
                        else if (segment.Type == SegmentType.Teleport)
                        {
                            DrawLine(segment, "Teleport");
                        }
                        else if (segment.Type == SegmentType.UI)
                        {
                            WrapLines(segment.Id, segment.Point1, segment.Point2.Value, Paints["UI"]);
                        }
                        else if (segment.Type == SegmentType.End)
                        {
                            DrawPoint(segment, "End");
                        }
                    }

                    byte alpha = 0;
                    for (int i = 0; i < gameState.Player.Ghosts.Count; i++)
                    {
                        alpha += (byte)(255 / gameState.Player.Ghosts.Count);
                        SKPaint ghostPaint = Paints["Player"].Clone();
                        ghostPaint.StrokeWidth = (i + 1) * (12 / gameState.Player.Ghosts.Count);
                        ghostPaint.Color = new SKColor(255, 255, 255, alpha);

                        Vector2 ghostPos = gameState.Player.Ghosts[i];
                        canvas.DrawPoint(ghostPos.X, ghostPos.Y, ghostPaint);
                    }

                    Vector2 playerPos = gameState.Player.Position;
                    canvas.DrawPoint(playerPos.X, playerPos.Y, Paints["Player"]);
                }
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
