using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// Class used when a method needs to get the active merchant and associated information.
    /// </summary>
    public class ActiveMerchantMBE 
    {
        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantID")]
        public Guid MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        /// <value>The merchants name</value>
        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public Uri LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets a list containing items
        /// </summary>
        [JsonPropertyName("catalogItems")]
        public List<CatalogItemMBE> CatalogItems { get; set; }

        
    }
}
