using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class represents an existing Merchant
    /// </summary>
    public class MerchantMBE : NewMerchantMBE
    {
        /// <summary>
        /// DB Generated PK for this Merchant.
        /// </summary>
        /// <value>The merchant identifier.</value>
        [JsonIgnore]
        public int MerchantId { get; set; }

        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantGuid")]
        public Guid MerchantGuid { get; init; }

        /// <summary>
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public Uri LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the logo file.
        /// </summary>
        /// <value>The name of the logo file.</value>
        /// <remarks>
        /// This property is from the DB, its value is dynamically converted to LogoUrl to be returned to the front end
        /// </remarks>
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
        public int? NumberOfDemoCustomers => DemoCustomers != null ? DemoCustomers.Count : null;

        /// <summary>
        /// Gets and sets a list of customers
        /// </summary>
        /// <returns>a list of customers</returns>
        [JsonPropertyName("customers")]
        public List<DemoCustomerMBE> DemoCustomers { get; set; }

    }
}
