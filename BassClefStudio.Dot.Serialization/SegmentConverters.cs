using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Serialization.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BassClefStudio.Dot.Serialization
{
    /// <summary>
    /// An <see cref="IToJsonConverter{T}"/> for the <see cref="Map"/> class.
    /// </summary>
    public class SegmentToJsonConverter : IToJsonConverter<Segment>
    {
        /// <inheritdoc/>
        public bool CanConvert(Segment item) => true;

        /// <inheritdoc/>
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

            for (int i = 0; i < item.Args.Length; i++)
            {
                properties.Add(new JProperty($"Arg{i}", item.Args[i]));
            }

            return new JObject(properties);
        }
    }

    /// <summary>
    /// An <see cref="IFromJsonConverter{T}"/> for the <see cref="Map"/> class.
    /// </summary>
    public class SegmentFromJsonConverter : IFromJsonConverter<Segment>
    {
        /// <inheritdoc/>
        public bool CanConvert(JToken item) => true;

        /// <inheritdoc/>
        public Segment Convert(JToken item)
        {
            if (item is JObject jObject)
            {
                Vector2 point1 = new Vector2((float)jObject["X1"], (float)jObject["Y1"]);
                string id = (string)jObject["Id"];

                var jsonArgs = jObject.Values()
                    .OfType<JProperty>()
                    .Where(j => j.Name.StartsWith("Arg"))
                    .OrderBy(j => j.Name);
                List<string> args = new List<string>();
                foreach (var a in jsonArgs)
                {
                    args.Add((string)a);
                }

                SegmentType type = (SegmentType)Enum.Parse(typeof(SegmentType), (string)jObject["Type"]);

                if ((item as JObject).ContainsKey("Y2"))
                {
                    Vector2 point2 = new Vector2((float)jObject["X2"], (float)jObject["Y2"]);
                    return new Segment(type, point1, point2, id, args.ToArray());
                }
                else
                {
                    return new Segment(type, point1, id, args.ToArray());
                }
            }
            else
            {
                throw new ArgumentException("JSON Segment values must always be JObjects.");
            }
        }
    }
}
