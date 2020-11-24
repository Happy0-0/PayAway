using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v0
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
    }
}