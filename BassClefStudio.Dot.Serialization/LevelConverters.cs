using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Serialization;
using BassClefStudio.Serialization.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Dot.Serialization
{
    /// <summary>
    /// An <see cref="IToJsonConverter{T}"/> for the <see cref="Level"/> class.
    /// </summary>
    public class LevelToJsonConverter : IToJsonConverter<Level>
    {
        /// <summary>
        /// The <see cref="IToJsonConverter{T}"/> for converting child <see cref="Segment"/>s.
        /// </summary>
        public IToJsonConverter<Segment> SegmentConverter { get; set; }

        /// <inheritdoc/>
        public bool CanConvert(Level item) => true;

        /// <inheritdoc/>
        public JToken Convert(Level item)
        {
            return new JObject(
                new JProperty("Map", new JArray(item.Segments.Select(s => SegmentConverter.GetTo(s)))),
                new JProperty("Name", item.Name));
        }
    }

    /// <summary>
    /// An <see cref="IFromJsonConverter{T}"/> for the <see cref="Level"/> class.
    /// </summary>
    public class LevelFromJsonConverter : IFromJsonConverter<Level>
    {
        /// <summary>
        /// The <see cref="IFromJsonConverter{T}"/> for converting child <see cref="Segment"/>s.
        /// </summary>
        public IFromJsonConverter<Segment> SegmentConverter { get; set; }

        /// <inheritdoc/>
        public bool CanConvert(JToken item) => true;

        /// <inheritdoc/>
        public Level Convert(JToken item)
        {
            return new Level((string)item["Name"], item["Map"].Select(s => SegmentConverter.GetTo(s)));
        }
    }
}
