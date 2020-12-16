using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Shared.Entities.v1
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "A non blank Merchant Name is required.")]
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
        /// <remarks>
        /// We cannot add this validation until the the UI tier adds this field
        /// </remarks>
        //[Required]
        //[Url(ErrorMessage = "You must supply a valid URL to a page on the merhant's web site")]
        public Uri MerchantUrl { get; set; }
    }
}