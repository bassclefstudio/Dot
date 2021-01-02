using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Graphics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    /// <summary>
    /// Represents the in-game camera, as implemented by the Dot Scratch project, and provides means for creating a <see cref="ViewCamera"/> for rendering Graphics content.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The size of the view, as a <see cref="Vector2"/> (used for adjusting zooms and bound pans).
        /// </summary>
        public Vector2 ViewSize { get; private set; }

        /// <summary>
        /// The scale of the <see cref="ViewSize"/> - in view-space to zoom-space (multiplier for zoom and bound pan calc.s).
        /// </summary>
        public float ViewScale { get; private set; }

        /// <summary>
        /// The <see cref="Vector2"/> position of the camera in game-space.
        /// </summary>
        public Vector2 CameraPosition { get; private set; }

        /// <summary>
        /// The <see cref="float"/> zoom of the camera in game-space.
        /// </summary>
        public float CameraScale { get; private set; }

        /// <summary>
        /// Creates and initializes a new <see cref="Camera"/>.
        /// </summary>
        public Camera()
        {
            CameraPosition = new Vector2(0, 0);
            CameraScale = 1;

            ViewSize = new Vector2(480, 360);
            ViewScale = 1;
        }

        /// <summary>
        /// Creates the <see cref="ViewCamera"/> used by the Graphics libraries with the information of this <see cref="Camera"/>.
        /// </summary>
        /// <param name="viewSpace">The true view-space size, which the <see cref="ViewCamera"/> adjusts for.</param>
        public ViewCamera GetGraphicsCamera(Vector2 viewSpace)
        {
            Vector2 drawSpace = new Vector2(480, 360);
            ZoomType zoomType = ZoomType.FitAll;
            var viewCamera = new ViewCamera(viewSpace, drawSpace, zoomType, true);
            var drawCamera = new ViewCamera(CameraScale, CameraPosition);

            return drawCamera + viewCamera;
        }

        #region Movement

        public void SetCamera(Vector2 pos, float scale)
        {
            CameraPosition = pos;
            CameraScale = scale;
        }

        public void LerpCamera(Vector2 pos, float scale, float lerpSpeed)
        {
            CameraPosition += (pos - CameraPosition) / lerpSpeed;
            CameraScale += (scale - CameraScale) / lerpSpeed;
        }

        public void SetView(Vector2 size, float scale)
        {
            ViewSize = size;
            ViewScale = scale;
        }

        #endregion
        #region Main

        public void MoveCamera(GameState gameState) => MoveCamera(gameState, 1, 1);
        public void MoveCamera(GameState gameState, float deltaFrames, float lerpSpeed)
        {
            var region = FindCamera(gameState);
            if (region == null)
            {
                LerpCamera(gameState.Player.Position, 1, lerpSpeed / deltaFrames);
            }
            else
            {
                Bounds bounds = new Bounds(region.Point1, region.Point2.Value);
                if (region.Args[0] == "Fixed")
                {
                    float zoom = region.ArgNums[1] ?? MinZoom(bounds);
                    LerpCamera(bounds.Center(), zoom, lerpSpeed / deltaFrames);
                }
                else if (region.Args[0] == "Bound")
                {
                    float zoom = region.ArgNums[1] ?? FullZoom(bounds);
                    LerpCamera(BoundPosition(gameState.Player.Position, bounds, zoom), zoom, lerpSpeed / deltaFrames);
                }
                else if (region.Args[0] == "View")
                {
                    float zoom = region.ArgNums[1] ?? FullZoom(bounds);
                    LerpCamera(gameState.Player.Position, zoom, lerpSpeed / deltaFrames);
                }
            }
        }

        #endregion
        #region Zoom

        private float MinZoom(Bounds bounds)
        {
            Vector2 size = bounds.Size();
            float xZoom = (ViewSize.X / ViewScale) / size.X;
            float yZoom = (ViewSize.Y / ViewScale) / size.Y;
            return xZoom < yZoom ? xZoom : yZoom;
        }

        private float FullZoom(Bounds bounds)
        {
            Vector2 size = bounds.Size();
            float xZoom = (ViewSize.X / ViewScale) / size.X;
            float yZoom = (ViewSize.Y / ViewScale) / size.Y;
            return xZoom < yZoom ? yZoom : xZoom;
        }

        #endregion
        #region Bounding
        
        private Vector2 BoundPosition(Vector2 pos, Bounds bounds, float camZoom)
        {
            Vector2 size = bounds.Size();
            float dX = (size.X - (ViewSize.X / (camZoom * ViewScale))) / 2;
            float dY = (size.Y - (ViewSize.Y / (camZoom * ViewScale))) / 2;

            dX = dX < 0 ? 0 : dX;
            dY = dY < 0 ? 0 : dY;

            Vector2 center = bounds.Center();
            Vector2 currentPos = pos;
            if(currentPos.X - center.X > dX)
            {
                currentPos.X = center.X + dX;
            }
            if (center.X - currentPos.X > dX)
            {
                currentPos.X = center.X - dX;
            }
            if (currentPos.Y - center.Y > dY)
            {
                currentPos.Y = center.Y + dY;
            }
            if (center.Y - currentPos.Y > dY)
            {
                currentPos.Y = center.Y - dY;
            }

            return currentPos;
        }

        #endregion
        #region CameraRegions

        private Segment FindCamera(GameState gameState)
        {
            return gameState.Map.CurrentLevel.Segments.Where(s => s.Type == SegmentType.Camera).LastOrDefault(s => InSegment(gameState.Player.Position, s));
        }
        
        private bool InSegment(Vector2 position, Segment segment)
        {
            Bounds bounds = new Bounds(segment.Point1, segment.Point2.Value);
            Vector2 size = bounds.Size();

            bool InBounds(Vector2 point)
            {
                return (Math.Abs(position.X - point.X) < size.X) && (Math.Abs(position.Y - point.Y) < size.Y);
            }

            return InBounds(segment.Point1) && InBounds(segment.Point2.Value);
        }

        #endregion
    }

    class Bounds
    {
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        public Vector2 Center()
        {
            return new Vector2((MinX + MaxX) / 2, (MinY + MaxY) / 2);
        }

        public Vector2 Size()
        {
            return new Vector2(MaxX - MinX, MaxY - MinY);
        }

        public Bounds(Vector2 a, Vector2 b)
        {
            var xs = new float[] { a.X, b.X };
            var ys = new float[] { a.Y, b.Y };

            MinX = xs.Min();
            MaxX = xs.Max();
            MinY = ys.Min();
            MaxY = ys.Max();
        }
    }
}
