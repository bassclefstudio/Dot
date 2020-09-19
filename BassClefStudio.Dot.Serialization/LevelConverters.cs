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
    public class LevelToJsonConverter : IToJsonConverter<Level>
    {
        public IToJsonConverter<Segment> SegmentConverter { get; set; }

        public bool CanConvert(Level item) => true;

        public JToken Convert(Level item)
        {
            return new JObject(
                new JProperty("Map", new JArray(item.Segments.Select(s => SegmentConverter.GetTo(s)))),
                new JProperty("Name", item.Name));
        }
    }

    public class LevelFromJsonConverter : IFromJsonConverter<Level>
    {
        public IFromJsonConverter<Segment> SegmentConverter { get; set; }

        public bool CanConvert(JToken item) => true;

        public Level Convert(JToken item)
        {
            return new Level((string)item["Name"], item["Map"].Select(s => SegmentConverter.GetTo(s)));
        }
    }
}
