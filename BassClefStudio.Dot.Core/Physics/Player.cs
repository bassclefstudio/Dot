using BassClefStudio.Dot.Core.Levels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace BassClefStudio.Dot.Core.Physics
{
    public class Player
    {
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public List<Vector2> Ghosts { get; } = new List<Vector2>();

        public Player()
        {
        }

        #region Physics
        #region Constants

        const float collisionDistance = 100;
        const int maxSlope = 8;
        const float drag = 0.1f;
        const int jumpDelay = 4;
        const float jumpSpeed = 12;
        const float moveSpeed = 1;
        const float minV = 0.25f;
        const float maxV = 18;
        const int maxGhosts = 5;

        #endregion
        #region Variables

        float frame = 0;
        int prevFrame = 0;
        float grounded = 0;

        #endregion
        #region Main

        public void DoPhysics(GameState gameState, float deltaFrames)
        {
            TempPos = Position;
            CalculateAcc();
            Velocity += Acceleration * deltaFrames;
            
            if(grounded > 0 && gameState.Inputs.Jump)
            {
                grounded = 0;
                Jump(jumpSpeed);
            }

            TempPos += ToVertical(Velocity) * deltaFrames;
            if(HandleWalls(gameState.Map.CurrentLevel.Segments, false) == CollisionHandled.Collision)
            {
                if ((Math.Sign(Velocity.X) == Math.Sign(Acceleration.X) || accCos == 0)
                    && (Math.Sign(Velocity.Y) == Math.Sign(Acceleration.Y) || accSin == 0))
                {
                    grounded = jumpDelay;
                }

                Velocity = ToHorizontal(Velocity);
            }
            else if(grounded > 0)
            {
                grounded -= deltaFrames;
            }

            Velocity += (deltaFrames * moveSpeed * gameState.Inputs.Move) * ToHorizontal(new Vector2(1, -1));
            TempPos += ToHorizontal(Velocity) * deltaFrames;
            if(HandleWalls(gameState.Map.CurrentLevel.Segments, true) == CollisionHandled.CollisionSlope)
            {
                TempPos -= ToHorizontal(Velocity) * deltaFrames;
                Velocity -= 1.5f * ToHorizontal(Velocity);
            }

            Velocity -= deltaFrames * drag * ToHorizontal(Velocity);
            if (Math.Abs(Velocity.X) > maxV)
            { Velocity = new Vector2(maxV * Math.Sign(Velocity.X), Velocity.Y); }
            if (Math.Abs(Velocity.Y) > maxV)
            { Velocity = new Vector2(Velocity.X, maxV * Math.Sign(Velocity.Y)); }
            if (Math.Abs(Velocity.X) < minV)
            { Velocity = new Vector2(0, Velocity.Y); }
            if (Math.Abs(Velocity.Y) < minV)
            { Velocity = new Vector2(Velocity.X, 0); }

            Position = TempPos;

            // Manage "ghosts"
            frame += deltaFrames;
            if (frame > 1)
            {
                frame = 0;
                if(Ghosts.Count > maxGhosts)
                {
                    Ghosts.RemoveAt(0);
                }

                Ghosts.Add(Position);
            }
        }

        #endregion
        #region Movement

        private enum CollisionHandled
        {
            NoCollision = 0,
            Collision = 1,
            CollisionSlope = 2
        }

        private CollisionHandled HandleWalls(IEnumerable<Segment> segments, bool useSlope)
        {
            var collision = GetCollision(segments.Where(s => s.Type == SegmentType.Wall), TempPos, collisionDistance);
            if(collision != null)
            {
                int slope = 0;
                bool isCollision = true;
                Vector2 perpendicular = -0.5f * new Vector2(
                    Math.Sign(Velocity.X) * Math.Sign(Acceleration.X) * accCos,
                    Math.Sign(Velocity.Y) * Math.Sign(Acceleration.Y) * accSin);
                Vector2 priorPos = TempPos;
                while((slope < maxSlope || !useSlope) && isCollision)
                {
                    slope += 1;
                    TempPos += perpendicular;
                    isCollision = GetCollision(collision, TempPos, collisionDistance);
                }

                if(isCollision)
                {
                    TempPos = priorPos;
                    return CollisionHandled.CollisionSlope;
                }
                else
                {
                    return CollisionHandled.Collision;
                }
            }
            else
            {
                return CollisionHandled.NoCollision;
            }
        }

        private void Jump(float speed)
        {
            Velocity = ToHorizontal(Velocity) * new Vector2(0, -1) + jumpSpeed * ToVertical(new Vector2(1, 1));
        }

        #endregion
        #region Vectors

        private Vector2 TempPos;

        public Vector2 Velocity { get; private set; } = new Vector2(0, 0);
        public Vector2 Acceleration { get; private set; } = new Vector2(0, -1);

        private float accCos;
        private float accSin;
        private float accMag;

        private void CalculateAcc()
        {
            accMag = Acceleration.Length();
            accCos = Acceleration.X / accMag;
            accSin = Acceleration.Y / accMag;
        }

        private Vector2 ToHorizontal(Vector2 xy)
        {
            return new Vector2(xy.X * Math.Abs(accSin), xy.Y * Math.Abs(accCos));
        }

        private Vector2 ToVertical(Vector2 xy)
        {
            return new Vector2(xy.X * Math.Abs(accCos), xy.Y * Math.Abs(accSin));
        }

        #endregion
        #region Collisions

        private bool GetCollision(Segment segment, Vector2 position, float distanceSquared)
        {
            return GetDistanceSquared(position, segment) < distanceSquared;
        }

        private Segment GetCollision(IEnumerable<Segment> segments, Vector2 position, float distanceSquared)
        {
            return segments.FirstOrDefault(s => GetDistanceSquared(position, s) < distanceSquared);
        }

        #endregion
        #region Distances

        private float GetDistanceSquared(Vector2 point, Segment segment)
        {
            if (segment.Point2.HasValue)
            {
                return GetDistanceSquared(point, segment.Point1, segment.Point2.Value);
            }
            else
            {
                return GetDistanceSquared(point, segment.Point1);
            }
        }

        private float GetDistanceSquared(Vector2 point, Vector2 line1, Vector2 line2)
        {
            float u = (((point.X - line1.X) * (line2.X - line1.X)) + ((point.Y - line1.Y) * (line2.Y - line1.Y))) / GetDistanceSquared(line1, line2);
            if(u > 1)
            {
                return GetDistanceSquared(line2, point);
            }
            else if (u < 0)
            {
                return GetDistanceSquared(line1, point);
            }
            else
            {
                Vector2 uVector = line1 + (line2 - line1) * u;
                return GetDistanceSquared(uVector, point);
            }
        }

        private float GetDistanceSquared(Vector2 a, Vector2 b)
        {
            return (a - b).LengthSquared();
        }

        #endregion
        #endregion
    }
}
