using PayAway.WebAPI.Entities.v1;
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
    public class OrderMBE : NewOrderMBE
    {
        /// <summary>
        /// Gets or sets order guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("orderID")]
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        /// <value>order number</value>
        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantID")]
        public Guid MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        /// <value>order date</value>
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets a list of order events
        /// </summary>
        [JsonPropertyName("orderEvents")]
        public List<OrderEventMBE> OrderEvents { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        /// <value>order total</value>
        [JsonPropertyName("orderTotal")]
        public decimal? OrderTotal 
        {
            get
            {
                return this.OrderLineItems != null ? this.OrderLineItems.Sum(oli => oli.ItemUnitPrice) : null;
            }

        }
                
    }
}
