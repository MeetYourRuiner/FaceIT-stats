using FaceitStats.Infrastructure.JSON.Strategies;
using Newtonsoft.Json.Linq;

namespace FaceitStats.Infrastructure.JSON
{
    class JsonParser
    {
        public IStrategy Strategy { get; set; }

        public JsonParser(IStrategy strategy)
        {
            Strategy = strategy;
        }

        public object Parse(JToken jToken)
        {
            return Strategy?.Parse(jToken);
        }
    }
}
