using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.Config
{
    /// <summary>
    /// This represents config info about the remote WebAPI server
    /// </summary>
    public class WebUrlConfigurationBE
    {
        /// <summary>The blazor base URL.</summary>
        /// <value>The blazor base URL.</value>
        [JsonPropertyName(@"hpp_baseurl")]
        public string HPPBaseUrl { get; set; }
    }
}
