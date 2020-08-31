using BassClefStudio.Dot.Core.Levels;
using Decklan.Serialization.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Serialization
{
    public class SegmentToJsonConverter : IToJsonConverter<Segment>
    {
        public bool CanConvert(Segment item) => true;

        public JToken Convert(Segment item)
        {
            List<JProperty> properties = new List<JProperty>();
            properties.Add(new JProperty("Type", item.Type.ToString()));
            properties.Add(new JProperty("X1", item.Point1.X));
            properties.Add(new JProperty("Y1", item.Point1.Y));
            if(item.Point2.HasValue)
            {
                properties.Add(new JProperty("X2", item.Point2.Value.X));
                properties.Add(new JProperty("Y2", item.Point2.Value.Y));
            }
            
            if(!string.IsNullOrWhiteSpace(item.Id))
            {
                properties.Add(new JProperty("Id", item.Id));
            }

            if (item.ArgNum.HasValue)
            {
                properties.Add(new JProperty("Args", item.Arg));
            }

            return new JObject(properties);
        }
    }

    public class SegmentFromJsonConverter : IFromJsonConverter<Segment>
    {
        public bool CanConvert(JToken item) => true;

        public Segment Convert(JToken item)
        {
            Vector2 point1 = new Vector2((float)item["X1"], (float)item["Y1"]);
            string id = (string)item["Id"];
            string arg = (string)item["Args"];
            SegmentType type = (SegmentType)Enum.Parse(typeof(SegmentType), (string)item["Type"]);

            if ((item as JObject).ContainsKey("Y2"))
            {
                Vector2 point2 = new Vector2((float)item["X2"], (float)item["Y2"]);
                return new Segment(type, point1, point2, id, arg);
            }
            else
            {
                return new Segment(type, point1, id, arg);
            }
        }
    }
}
