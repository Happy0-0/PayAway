using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class used when a method needs to get the active merchant and associated information.
    /// </summary>
    public record ActiveMerchantMBE 
    {
        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantGuid")]
        public Guid MerchantGuid { get; init; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        /// <value>The merchants name</value>
        [JsonPropertyName("merchantName")]
        public string MerchantName { get; init; }

        /// <summary>
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public Uri LogoUrl { get; init; }

        /// <summary>
        /// Gets or sets a list containing Catalog Items
        /// </summary>
        [JsonPropertyName("catalogItems")]
        public List<CatalogItemMBE> CatalogItems { get; init; }

        /// <summary>
        /// Gets or sets a list containing Demo Customers
        /// </summary>
        [JsonPropertyName("demoCustomers")]
        public List<DemoCustomerMBE> DemoCustomers { get; init; }
    }
}
