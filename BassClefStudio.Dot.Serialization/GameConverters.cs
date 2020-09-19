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
    public class GameToJsonConverter : IToJsonConverter<Map>
    {
        public IToJsonConverter<Level> LevelConverter { get; set; }

        public bool CanConvert(Map item) => true;

        public JToken Convert(Map item)
        {
            return new JArray(item.Levels.Select(l => LevelConverter.GetTo(l)));
        }
    }

    public class GameFromJsonConverter : IFromJsonConverter<Map>
    {
        public IFromJsonConverter<Level> LevelConverter { get; set; }

        public bool CanConvert(JToken item) => true;

        public Map Convert(JToken item)
        {
            return new Map(item.Select(l => LevelConverter.GetTo(l)));
        }
    }
}
