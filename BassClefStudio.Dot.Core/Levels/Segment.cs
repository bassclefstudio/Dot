using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace BassClefStudio.Dot.Core.Levels
{
    /// <summary>
    /// Represents a single line or point in the game.
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// A <see cref="SegmentType"/> value indicating the function of the <see cref="Segment"/> data.
        /// </summary>
        public SegmentType Type { get; }

        /// <summary>
        /// The first point, either the center of a point or the start of a line.
        /// </summary>
        public Vector2 Point1 { get; }

        /// <summary>
        /// If not null, the end of a line.
        /// </summary>
        public Vector2? Point2 { get; }

        /// <summary>
        /// A <see cref="string"/> ID, unique to the level, identifying this <see cref="Segment"/>.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A collection of <see cref="string"/> arguments representing additional parameters.
        /// </summary>
        public string[] Args { get; }

        /// <summary>
        /// The <see cref="Args"/> array, but with all able arguments parsed as <see cref="float"/> numbers.
        /// </summary>
        public float?[] ArgNums { get; }

        /// <summary>
        /// Creates a new point <see cref="Segment"/>.
        /// </summary>
        /// <param name="type">A <see cref="SegmentType"/> value indicating the function of the <see cref="Segment"/> data.</param>
        /// <param name="point1">The center of the point <see cref="Segment"/>.</param>
        /// <param name="id">A <see cref="string"/> ID, unique to the level, identifying this <see cref="Segment"/>.</param>
        /// <param name="args">A collection of <see cref="string"/> arguments representing additional parameters.</param>
        public Segment(SegmentType type, Vector2 point1, string id = null, params string[] args)
        {
            Type = type;
            Point1 = point1;
            Id = id;
            Args = args;
            ArgNums = Args.Select(a => float.TryParse(a, out var f) ? f : (float?)null).ToArray();
        }

        /// <summary>
        /// Creates a new line <see cref="Segment"/>.
        /// </summary>
        /// <param name="type">A <see cref="SegmentType"/> value indicating the function of the <see cref="Segment"/> data.</param>
        /// <param name="point1">The start of the line <see cref="Segment"/>.</param>
        /// <param name="point2">The end of the line <see cref="Segment"/>.</param>
        /// <param name="id">A <see cref="string"/> ID, unique to the level, identifying this <see cref="Segment"/>.</param>
        /// <param name="args">A collection of <see cref="string"/> arguments representing additional parameters.</param>
        public Segment(SegmentType type, Vector2 point1, Vector2 point2, string id = null, params string[] args)
            : this(type, point1, id, args)
        {
            Point2 = point2;
        }
    }

    /// <summary>
    /// An enum representing the type of <see cref="Segment"/> the data represents.
    /// </summary>
    public enum SegmentType
    {
        /// <summary>
        /// A solid wall/floor segment.
        /// </summary>
        Wall = 0,

        /// <summary>
        /// Touching the segment kills the player.
        /// </summary>
        Lava = 1,

        /// <summary>
        /// Touching the segment sends the player bouncing.
        /// </summary>
        Bounce = 2,

        /// <summary>
        /// Passing through the segment changes the behavior of gravity.
        /// </summary>
        Flip = 3,

        /// <summary>
        /// A point portal that can transport the player.
        /// </summary>
        Portal = 4,

        /// <summary>
        /// A teleport ('line portal') that can transport the player.
        /// </summary>
        Teleport = 5,

        /// <summary>
        /// Displays text on-screen that the player does not interact with.
        /// </summary>
        UI = 6,

        /// <summary>
        /// Represents camera metadata such as zoom/pan instructions.
        /// </summary>
        Camera = 7,

        /// <summary>
        /// Indicates a spawn position of a player.
        /// </summary>
        Start = 8,

        /// <summary>
        /// Touching the point sends the player to a different level.
        /// </summary>
        End = 9
    }
}
