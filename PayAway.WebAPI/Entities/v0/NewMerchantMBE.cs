using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// Class used when creating new merchants
    /// </summary>
    public class NewMerchantMBE
    {
        /// <summary>
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        /// <value>The merchants name</value>
        [JsonPropertyName("merchantName")]
        public string MerchantName { get; set; }

        /// <summary>
        /// True if the merchant is active
        /// </summary>
        /// <value>True</value>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// True if the merchant supports tips
        /// </summary>
        /// <value>True</value>
        [JsonPropertyName("isSupportsTips")]
        public bool IsSupportsTips { get; set; }
    }
}