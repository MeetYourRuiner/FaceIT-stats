using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace FaceitStats.Infrastructure.Data.Config
{
    abstract class BaseMappingContractResolver : DefaultContractResolver
    {
        protected Dictionary<string, string> PropertyMappings { get; set; }

        public BaseMappingContractResolver()
        {
            this.PropertyMappings = SetPropertyMappings();
            //    new Dictionary<string, string>
            //{
            //    {"Meta", "meta"},
            //    {"LastUpdated", "last_updated"},
            //    {"Disclaimer", "disclaimer"},
            //    {"License", "license"},
            //    {"CountResults", "results"},
            //    {"Term", "term"},
            //    {"Count", "count"},
            //};
        }

        protected abstract Dictionary<string, string> SetPropertyMappings();

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
