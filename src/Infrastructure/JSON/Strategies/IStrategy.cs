using Newtonsoft.Json.Linq;

namespace FaceitStats.Infrastructure.JSON.Strategies
{
    interface IStrategy
    {
        object Parse(JToken jToken);
    }
}
