using BassClefStudio.Dot.Core.Levels;
using Decklan.Serialization.DI;
using Newtonsoft.Json.Linq;
using Decklan.Serialization.Json;
using Autofac;
using System.Reflection;
using Decklan.Serialization;

namespace BassClefStudio.Dot.Serialization
{
    public class JsonGameService : ConverterService<Map, JToken, JToken>
    {
        public JsonGameService() : base(typeof(JsonGameService).GetTypeInfo().Assembly)
        { }

        public override Map ReadItem(JToken input)
        {
            return ConverterContainer.Resolve<IFromJsonConverter<Map>>().GetTo(input);
        }

        public override JToken WriteItem(Map item)
        {
            return ConverterContainer.Resolve<IToJsonConverter<Map>>().GetTo(item);
        }
    }
}
