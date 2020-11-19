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
    public class NewOrderMBE
    {
        /// <summary>
        /// Gets or sets the orders customer name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the orders customers phone number
        /// </summary>
        /// <value>phone number</value>
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a list of items
        /// </summary>
        [JsonPropertyName("orderItems")]
        public List<NewOrderLineItemMBE> OrderLineItems { get; set; }
    }
}
