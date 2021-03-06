﻿using BassClefStudio.Dot.Core.Levels;
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
        /// <summary>
        /// The current position of the <see cref="Player"/>.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The previous 'ghosted' positions of the <see cref="Player"/>.
        /// </summary>
        public List<Vector2> Ghosts { get; } = new List<Vector2>();

        /// <summary>
        /// Creates a new <see cref="Player"/> and resets it.
        /// </summary>
        public Player()
        {
            Reset();
        }

        /// <summary>
        /// Resets the <see cref="Player"/> to its initial state.
        /// </summary>
        public void Reset()
        {
            Ghosts.Clear();
            Position = new Vector2(0, 0);
            grounded = 0;
            frame = 0;
            Velocity = new Vector2(0, 0);
            Acceleration = new Vector2(0, -1);

            jumping = false;
            inPortal = false;
            inEnd = false;
            isFlip = null;
        }

        public void SetStartingPos(Level level)
        {
            Reset();
            var start = level.Segments.FirstOrDefault(s => s.Type == SegmentType.Start);
            if (start != null)
            {
                Position = start.Point1;
            }
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
        const float maxV = 16.5f;
        const int maxGhosts = 5;

        #endregion
        #region Variables

        float frame;
        float grounded;

        #endregion
        #region Main

        /// <summary>
        /// Applies the physics calculations to the given <see cref="GameState"/>.
        /// </summary>
        /// <param name="gameState">The <see cref="GameState"/> to move the <see cref="Player"/> against.</param>
        /// <param name="deltaFrames">The number of frames passed since the last call to <see cref="DoPhysics(GameState, float)"/>.</param>
        public void DoPhysics(GameState gameState, float deltaFrames)
        {
            TempPos = Position;
            CalculateAcc();
            Velocity += Acceleration * deltaFrames;

            if (grounded > 0 && gameState.Inputs.Jump)
            {
                grounded = 0;
                Jump(jumpSpeed);
            }

            TempPos += ToVertical(Velocity) * deltaFrames;
            if (HandleWalls(gameState.Map.CurrentLevel.Segments, deltaFrames, false) == CollisionHandled.Collision)
            {
                if ((Math.Sign(Velocity.X) == Math.Sign(Acceleration.X) || accCos == 0)
                    && (Math.Sign(Velocity.Y) == Math.Sign(Acceleration.Y) || accSin == 0))
                {
                    grounded = jumpDelay;
                }

                Velocity = ToHorizontal(Velocity);
            }
            else if (grounded > 0)
            {
                grounded -= deltaFrames;
            }

            Velocity += (deltaFrames * moveSpeed * gameState.Inputs.Move) * ToHorizontal(new Vector2(1, -1));
            TempPos += ToHorizontal(Velocity) * deltaFrames;
            if (HandleWalls(gameState.Map.CurrentLevel.Segments, deltaFrames, true) == CollisionHandled.CollisionSlope)
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

            CheckLavaPlatforms(gameState);
            CheckJumpPlatforms(gameState);
            CheckPortals(gameState);
            CheckFlip(gameState);
            CheckEnd(gameState);

            ManageGhosts(deltaFrames);
        }

        private void ManageGhosts(float deltaFrames)
        {
            //// Manage "ghosts"
            frame += deltaFrames;
            //// Run at 60fps (1 ghost per 2 frames)
            if (frame >= 1)
            {
                frame = 0;
                Ghosts.Add(Position);
                if (Ghosts.Count > maxGhosts)
                {
                    Ghosts.RemoveAt(0);
                }
            }
        }

        #endregion
        #region Platforms

        private bool jumping;
        public void CheckJumpPlatforms(GameState gameState)
        {
            var segments = gameState.Map.CurrentLevel.Segments;
            Segment collision = GetCollision(
                segments.Where(s => s.Type == SegmentType.Bounce),
                Position,
                collisionDistance);
            bool isCollision = collision != null;

            if (isCollision && !jumping)
            {
                Jump(1.5f * jumpSpeed);
            }
            jumping = isCollision;
        }

        public void CheckLavaPlatforms(GameState gameState)
        {
            var segments = gameState.Map.CurrentLevel.Segments;
            Segment collision = GetCollision(
                segments.Where(s => s.Type == SegmentType.Lava),
                Position,
                collisionDistance);
            bool isCollision = collision != null;

            if (isCollision)
            {
                Reset();
                gameState.ResetLevel();
            }
        }

        private bool inPortal;
        public void CheckPortals(GameState gameState)
        {
            var segments = gameState.Map.CurrentLevel.Segments;

            void SetPosAndVel(Segment portal)
            {
                var otherId = portal.Args.ElementAtOrDefault(0);
                if (otherId != null)
                {
                    var otherPortal = segments.FirstOrDefault(
                        s => (s.Type == SegmentType.Portal
                        || s.Type == SegmentType.Teleport)
                        && s.Id == otherId);

                    if (otherPortal != null)
                    {
                        //To point portal.
                        if (otherPortal.Type == SegmentType.Portal)
                        {
                            Position = otherPortal.Point1;
                        }
                        //To line portal.
                        else if (otherPortal.Type == SegmentType.Teleport)
                        {
                            float angle = 0f;
                            Vector2 slope = otherPortal.Point2.Value - otherPortal.Point1;

                            //Line to line.
                            if (portal.Type == SegmentType.Teleport)
                            {
                                float u = GetU(Position, portal.Point1, portal.Point2.Value);
                                Position = GetBetween(otherPortal.Point1, otherPortal.Point2.Value, u);
                                angle = GetAngle(portal.Point2.Value - portal.Point1) - GetAngle(slope);
                            }
                            //Point to line.
                            else if (portal.Type == SegmentType.Portal)
                            {
                                Position = GetBetween(otherPortal.Point1, otherPortal.Point2.Value, 0.5f);
                                angle = ((float)Math.PI / 2) + GetAngle(slope);
                            }

                            Velocity = Rotate(Velocity, angle);
                        }
                    }
                }
            }

            Segment portalCollision = GetCollision(
                segments.Where(s => s.Type == SegmentType.Portal),
                Position,
                collisionDistance * 2);

            if (portalCollision != null)
            {
                if (!inPortal)
                {
                    SetPosAndVel(portalCollision);
                }
                inPortal = true;
            }
            else
            {
                Segment teleportCollision = GetCollision(
                    segments.Where(s => s.Type == SegmentType.Teleport),
                    Position,
                    collisionDistance);

                if (teleportCollision != null)
                {
                    if (!inPortal)
                    {
                        SetPosAndVel(teleportCollision);
                    }
                    inPortal = true;
                }
                else
                {
                    inPortal = false;
                }
            }
        }

        private bool inEnd;
        public void CheckEnd(GameState gameState)
        {
            var segments = gameState.Map.CurrentLevel.Segments;
            Segment collision = GetCollision(
                segments.Where(s => s.Type == SegmentType.End),
                Position,
                collisionDistance * 2);
            bool isCollision = collision != null;

            if (isCollision && !inEnd)
            {
                if (string.IsNullOrWhiteSpace(collision.Args.ElementAtOrDefault(0)))
                {
                    gameState.Map.NextLevel();
                }
                else
                {
                    var level = gameState.Map.Levels.FirstOrDefault(l => l.Name == collision.Args[0]);
                    if (level == null)
                    {
                        gameState.Map.NextLevel();
                    }
                    else
                    {
                        gameState.Map.SetLevel(level);
                    }
                }
            }
            inEnd = isCollision;
        }

        bool? isFlip = null;
        public void CheckFlip(GameState gameState)
        {
            float maxCheck = Velocity.LengthSquared();
            if (maxCheck < collisionDistance)
            {
                maxCheck = collisionDistance;
            }

            var segments = gameState.Map.CurrentLevel.Segments;
            Segment collision = GetCollision(
                segments.Where(s => s.Type == SegmentType.Flip),
                Position,
                maxCheck);

            if (collision != null)
            {
                float u = GetU(Position, collision.Point1, collision.Point2.Value);
                if (u > 0 && u < 1)
                {
                    bool side = GetSide(Position, collision.Point1, collision.Point2.Value);
                    if(isFlip != null && isFlip != side)
                    {
                        Acceleration *= new Vector2(1, -1);
                    }
                    isFlip = side;
                }
                else
                {
                    isFlip = null;
                }
            }
            else
            {
                isFlip = null;
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

        private CollisionHandled HandleWalls(IEnumerable<Segment> segments, float deltaFrames, bool useSlope)
        {
            var collision = GetCollision(segments.Where(s => s.Type == SegmentType.Wall), TempPos, collisionDistance);
            if (collision != null)
            {
                int slope = 0;
                bool isCollision = true;
                Vector2 perpendicular = -0.5f * new Vector2(
                    Math.Sign(Velocity.X) * Math.Sign(Acceleration.X) * accCos,
                    Math.Sign(Velocity.Y) * Math.Sign(Acceleration.Y) * accSin);
                Vector2 priorPos = TempPos;
                while ((slope < maxSlope || !useSlope) && isCollision)
                {
                    slope += 1;
                    TempPos += deltaFrames * perpendicular;
                    isCollision = GetCollision(collision, TempPos, collisionDistance);
                }

                if (slope >= maxSlope && useSlope)
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
            Velocity = new Vector2(1, -1) * (ToHorizontal(Velocity) + new Vector2(speed * accCos, speed * accSin));
        }

        #endregion
        #region Vectors

        /// <summary>
        /// A temporary store for the <see cref="Position"/> of the <see cref="Player"/> during calculations.
        /// </summary>
        private Vector2 TempPos;

        /// <summary>
        /// The current <see cref="Vector2"/> velocity of the <see cref="Player"/>.
        /// </summary>
        public Vector2 Velocity { get; private set; }
        /// <summary>
        /// The current <see cref="Vector2"/> acceleration of the <see cref="Player"/>.
        /// </summary>
        public Vector2 Acceleration { get; private set; }

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
            return new Vector2(Math.Abs(accSin), Math.Abs(accCos)) * xy;
        }

        private Vector2 ToVertical(Vector2 xy)
        {
            return new Vector2(Math.Abs(accCos), Math.Abs(accSin)) * xy;
        }

        private float GetAngle(Vector2 slope)
        {
            if (slope.Y == 0)
            {
                return slope.X < 0 ? (float)Math.PI / 2 : (float)-Math.PI / 2;
            }
            else
            {
                return slope.Y < 0 ? (float)Math.Atan(slope.X / slope.Y) : (float)(Math.PI + Math.Atan(slope.X / slope.Y));
            }
        }

        private Vector2 GetBetween(Vector2 a, Vector2 b, float u)
        {
            return a + u * (b - a);
        }

        private Vector2 Rotate(Vector2 v, float angle)
        {
            return new Vector2((float)((v.X * Math.Cos(angle)) - (v.Y * Math.Sin(angle))), (float)((v.X * Math.Sin(angle)) + (v.Y * Math.Cos(angle))));
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

        private float GetU(Vector2 point, Vector2 line1, Vector2 line2)
        {
            return (((point.X - line1.X) * (line2.X - line1.X)) + ((point.Y - line1.Y) * (line2.Y - line1.Y))) / GetDistanceSquared(line1, line2);
        }

        private float GetDistanceSquared(Vector2 point, Vector2 line1, Vector2 line2)
        {
            float u = GetU(point, line1, line2);
            if (u > 1)
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

        private bool GetSide(Vector2 point, Vector2 line1, Vector2 line2)
        {
            var b = (point.X - line1.X) * (line2.Y - line1.Y) - (point.Y - line1.Y) * (line2.X - line1.X);
            return b > 0;

        }

        #endregion
        #endregion
    }
}
