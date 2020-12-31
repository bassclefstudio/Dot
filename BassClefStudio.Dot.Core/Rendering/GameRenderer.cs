using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Physics;
using BassClefStudio.Graphics.Core;
using BassClefStudio.Graphics.Turtle;
using BassClefStudio.NET.Core.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    /// <summary>
    /// A service for drawing a Dot <see cref="Core.GameState"/>.
    /// </summary>
    public class GameRenderer
    {
        public GameState GameState { get; set; }

        public Dictionary<string, Color> Paints { get; }

        public Camera ViewCamera { get; set; }

        /// <summary>
        /// Creates a new <see cref="GameRenderer"/>.
        /// </summary>
        public GameRenderer()
        {
            Paints = new Dictionary<string, Color>()
            {
                { "Background", new Color(0,200,0) },
                { "Wall", new Color(255,255,255) },
                { "Lava", new Color(255,0,0) },
                { "Bounce", new Color(200,200,0) },
                { "Flip", new Color(255,40,255) },
                { "Portal", new Color(200,200,255) },
                { "Teleport", new Color(200,200,255) },
                { "End", new Color(100,255,100) },
                { "Player", new Color(255,255,255) }
            };
        }

        public void Render(ITurtleGraphicsProvider graphics, Vector2 viewSize)
        {
            graphics.Camera = ViewCamera.GetGraphicsCamera(viewSize);
            graphics.PenSize = 10;
            if (GameState.Map != null)
            {
                if (GameState.Map.CurrentLevel != null)
                {
                    void DrawLine(Segment segment, string paintKey)
                    {
                        Vector2 p1 = segment.Point1;
                        Vector2 p2 = segment.Point2.Value;
                        graphics.DrawLine(
                            segment.Point1,
                            segment.Point2.Value,
                            Paints[paintKey]);
                    }

                    void DrawPoint(Segment segment, float size, string paintKey)
                    {
                        graphics.FillEllipse(
                            segment.Point1,
                            new Vector2(size, size),
                            Paints[paintKey]);
                    }

                    foreach (var segment in GameState.Map.CurrentLevel.Segments)
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
                            DrawPoint(segment, 8, "Portal");
                        }
                        else if (segment.Type == SegmentType.Teleport)
                        {
                            DrawLine(segment, "Teleport");
                        }
                        //else if (segment.Type == SegmentType.UI)
                        //{
                        //    WrapLines(segment.Id, segment.Point1, segment.Point2.Value, Paints["UI"]);
                        //}
                        else if (segment.Type == SegmentType.End)
                        {
                            DrawPoint(segment, 8, "End");
                        }
                    }

                    byte alpha = 0;
                    for (int i = 0; i < GameState.Player.Ghosts.Count; i++)
                    {
                        alpha += (byte)(255 / GameState.Player.Ghosts.Count);
                        var ghostWidth = (i + 1) * (6 / GameState.Player.Ghosts.Count);
                        var ghostColor = new Color(255, 255, 255, alpha);

                        Vector2 ghostPos = GameState.Player.Ghosts[i];
                        graphics.FillEllipse(ghostPos, new Vector2(ghostWidth, ghostWidth), ghostColor);
                    }

                    Vector2 playerPos = GameState.Player.Position;
                    graphics.FillEllipse(playerPos, new Vector2(4, 4), Paints["Player"]);
                }
                _ = graphics.FlushAsync();
            }
        }
    }
}
