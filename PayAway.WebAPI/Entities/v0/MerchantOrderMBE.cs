using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// Class used when a method needs to get merchant order information
    /// </summary>
    public class MerchantOrderMBE
    {
        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantID")]
        public Guid MerchantID { get; set; }

        /// <summary>
        /// Gets or sets the order name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order phone number
        /// </summary>
        /// <value>phone number</value>
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        /// <value>order total</value>
        [JsonPropertyName("orderTotal")]
        public string OrderTotal { get; set; }
    }
}
