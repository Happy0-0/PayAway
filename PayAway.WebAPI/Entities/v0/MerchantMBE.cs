using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// class used when a method needs to get merchant information. Inherits from NewMerchantMBE.
    /// </summary>
    public class MerchantMBE : NewMerchantMBE
    {

        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantGuid")]
        public Guid MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public Uri LogoUrl { get; set; }

        [JsonIgnore]
        public string LogoFileName { get; set; }

        /// <summary>
        /// True if the merchant is active
        /// </summary>
        /// <value>True</value>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the number of customers a merchant has
        /// </summary>
        /// <value>number of customers</value>
        [JsonPropertyName("numberOfCustomers")]
        public int? NumberOfDemoCustomers 
        {
            get { return DemoCustomers != null ? DemoCustomers.Count : null; }
        }

        /// <summary>
        /// Gets and sets a list of customers
        /// </summary>
        /// <returns>a list of customers</returns>
        [JsonPropertyName("customers")]
        public List<CustomerMBE> DemoCustomers { get; set; }

    }
}
