using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Serialization.DI;
using Newtonsoft.Json.Linq;
using BassClefStudio.Serialization.Json;
using Autofac;
using System.Reflection;
using BassClefStudio.Serialization;

namespace BassClefStudio.Dot.Serialization
{
    /// <summary>
    /// A <see cref="ConverterService{TItem, TInput, TOutput}"/> designed for reading and writing Dot game files (as <see cref="Map"/>s).
    /// </summary>
    public class JsonGameService : ConverterService<Map, JToken, JToken>
    {
        /// <summary>
        /// Creates a new <see cref="JsonGameService"/>.
        /// </summary>
        public JsonGameService() : base(typeof(JsonGameService).GetTypeInfo().Assembly)
        { }

        /// <inheritdoc/>
        public override Map ReadItem(JToken input)
        {
            return ConverterContainer.Resolve<IFromJsonConverter<Map>>().GetTo(input);
        }

        /// <inheritdoc/>
        public override JToken WriteItem(Map item)
        {
            return ConverterContainer.Resolve<IToJsonConverter<Map>>().GetTo(item);
        }
    }
}
