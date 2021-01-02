using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Serialization;
using BassClefStudio.Serialization.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.Dot.Serialization
{
    /// <summary>
    /// An <see cref="IToJsonConverter{T}"/> for the <see cref="Map"/> class.
    /// </summary>
    public class GameToJsonConverter : IToJsonConverter<Map>
    {
        /// <summary>
        /// The <see cref="IToJsonConverter{T}"/> for converting child <see cref="Level"/>s.
        /// </summary>
        public IToJsonConverter<Level> LevelConverter { get; set; }

        /// <inheritdoc/>
        public bool CanConvert(Map item) => true;

        /// <inheritdoc/>
        public JToken Convert(Map item)
        {
            return new JObject(
                new JProperty("Levels", new JArray(item.Levels.Select(l => LevelConverter.GetTo(l)))),
                new JProperty("Version", Map.Version));
        }
    }

    /// <summary>
    /// An <see cref="IFromJsonConverter{T}"/> for the <see cref="Map"/> class.
    /// </summary>
    public class GameFromJsonConverter : IFromJsonConverter<Map>
    {
        /// <summary>
        /// The <see cref="IFromJsonConverter{T}"/> for converting child <see cref="Level"/>s.
        /// </summary>
        public IFromJsonConverter<Level> LevelConverter { get; set; }

        /// <inheritdoc/>
        public bool CanConvert(JToken item) => true;

        /// <inheritdoc/>
        public Map Convert(JToken item)
        {
            if (item.Type == JTokenType.Object && (item as JObject).ContainsKey("Version"))
            {
                var map = new Map(item["Levels"].Select(l => LevelConverter.GetTo(l)));

                int version = (int)item["Version"];
                while (version != Map.Version)
                {
                    version = UpdateMap(version, map);
                }

                return map;
            }
            else
            {
                throw new SerializationException("Failed to deserialize game - JSON schema does not contain required versioning parameters of v4.5 and above.");
            }
        }
        
        /// <summary>
        /// Updated the information saved in the given <see cref="Map"/> from the given <see cref="int"/> schema version.
        /// </summary>
        /// <param name="currentVersion">The current <see cref="Map"/> schema version (<see cref="Map.Version"/> is the desired max version).</param>
        /// <param name="currentMap">The currently-saved <see cref="Map"/> data in the <paramref name="currentVersion"/> format.</param>
        /// <returns>The now-current <see cref="int"/> version of the schema the <see cref="Map"/> data now represents.</returns>
        internal int UpdateMap(int currentVersion, Map currentMap)
        {
            throw new NotImplementedException();
        }
    }
}
