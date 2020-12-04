using System;
using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class represents a new or updated merchant
    /// </summary>
    public class NewMerchantMBE
    {
        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        /// <value>The merchants name</value>
        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        /// <summary>
        /// True if the merchant supports tips
        /// </summary>
        /// <value>True</value>
        [JsonPropertyName("isSupportsTips")]
        public bool IsSupportsTips { get; set; }

        /// <summary>
        /// Gets or sets the merchant Url
        /// </summary>
        /// <value>the merchant url</value>
        public Uri MerchantUrl { get; set; }
    }
}