using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Shared.Entities.v1
{
    /// <summary>
    /// Class represents a new or updated order
    /// </summary>
    public class NewOrderMBE
    {
        /// <summary>
        /// Gets or sets the orders customer name
        /// </summary>
        [JsonPropertyName("name")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the orders customers phone number
        /// </summary>
        /// <value>phone number</value>
        [JsonPropertyName("phoneNumber")]
        public string CustomerPhoneNo { get; set; }

        /// <summary>
        /// Gets or sets a list of items
        /// </summary>
        [JsonPropertyName("orderItems")]
        public List<NewOrderLineItemMBE> OrderLineItems { get; set; }
    }
}
