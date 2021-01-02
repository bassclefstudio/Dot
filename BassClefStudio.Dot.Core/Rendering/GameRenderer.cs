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
        /// <summary>
        /// The current game to render.
        /// </summary>
        public GameState GameState { get; set; }

        /// <summary>
        /// A collection of <see cref="Color"/>/stroke-width pairs for drawing similar lines/points.
        /// </summary>
        public Dictionary<string, Tuple<Color, float?>> Paints { get; }

        /// <summary>
        /// The <see cref="Camera"/> used to view the <see cref="GameState"/>.
        /// </summary>
        public Camera ViewCamera { get; set; }

        /// <summary>
        /// Creates a new <see cref="GameRenderer"/>.
        /// </summary>
        public GameRenderer()
        {
            Paints = new Dictionary<string, Tuple<Color, float?>>()
            {
                { "Background", new Tuple<Color, float?>(new Color(0,200,0), null) },
                { "Wall", new Tuple<Color, float?>(new Color(255,255,255), null) },
                { "Lava", new Tuple<Color, float?>(new Color(255,0,0), null) },
                { "Bounce", new Tuple<Color, float?>(new Color(200,200,0), null) },
                { "Flip", new Tuple<Color, float?>(new Color(255,40,255), 6) },
                { "Portal", new Tuple<Color, float?>(new Color(200,200,255), null) },
                { "Teleport", new Tuple<Color, float?>(new Color(200,200,255), 6) },
                { "End", new Tuple<Color, float?>(new Color(100,255,100), null) },
                { "Player", new Tuple<Color, float?>(new Color(255,255,255), null) }
            };
        }

        /// <summary>
        /// Renders the content of the <see cref="GameState"/> onto an <see cref="ITurtleGraphicsProvider"/>.
        /// </summary>
        /// <param name="graphics">An <see cref="ITurtleGraphicsProvider"/> to draw the game onto.</param>
        /// <param name="viewSize">The true view-size to adjust for.</param>
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
                            Paints[paintKey].Item1,
                            Paints[paintKey].Item2);
                    }

                    void DrawPoint(Segment segment, float size, string paintKey)
                    {
                        graphics.FillEllipse(
                            segment.Point1,
                            new Vector2(size, size),
                            Paints[paintKey].Item1);
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

                    byte alpha = 128;
                    for (int i = 0; i < GameState.Player.Ghosts.Count; i++)
                    {
                        alpha += (byte)(127 / GameState.Player.Ghosts.Count);
                        var ghostWidth = (i + 1) * (6 / GameState.Player.Ghosts.Count);
                        var ghostColor = new Color(255, 255, 255, alpha);

                        Vector2 ghostPos = GameState.Player.Ghosts[i];
                        graphics.FillEllipse(ghostPos, new Vector2(ghostWidth), ghostColor);
                    }

                    Vector2 playerPos = GameState.Player.Position;
                    graphics.FillEllipse(playerPos, new Vector2(6), Paints["Player"].Item1);
                }
                _ = graphics.FlushAsync();
            }
        }
    }
}
