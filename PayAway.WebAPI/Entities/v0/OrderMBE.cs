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
    public class OrderMBE
    {
        /// <summary>
        /// Gets or sets order guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("orderGuid")]
        public Guid OrderGuid { get; init; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        /// <value>order number</value>
        [JsonPropertyName("orderNumber")]
        public string OrderNumber => this.OrderId.ToString("0000");

        [JsonIgnore]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("status")]
        public Enums.ORDER_STATUS OrderStatus { get; set; }

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
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantGuid")]
        public Guid MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        /// <value>order date</value>
        [JsonPropertyName("orderDateTimeUTC")]
        public DateTime OrderDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets a list of order events
        /// </summary>
        [JsonPropertyName("orderEvents")]
        public List<OrderEventMBE> OrderEvents { get; set; }
                
        /// <summary>
        /// Gets or sets a list of items
        /// </summary>
        [JsonPropertyName("orderItems")]
        public List<CatalogItemMBE> OrderLineItems { get; set; }

        /// <summary>
        /// Indicates if Sending the payment link is available based on the current status of the order
        /// </summary>
        [JsonPropertyName("isSendPaymentLinkAvailable")]
        public bool IsSendPaymentLinkAvailable => this.OrderStatus != Enums.ORDER_STATUS.Paid;

        /// <summary>
        /// Indicates if updates are allowed based on the current status of the order
        /// </summary>
        [JsonPropertyName("isUpdateAvailable")]
        public bool IsUpdateAvailable
        {
            get { return (this.OrderStatus != Enums.ORDER_STATUS.SMS_Sent && this.OrderStatus != Enums.ORDER_STATUS.Paid); }
        }

        /// <summary>
        /// Gets or sets the order total
        /// </summary> 
        /// <value>order total</value>
        [JsonPropertyName("orderTotal")]
        public decimal? OrderTotal 
        {
            get
            {
                return this.OrderLineItems?.Sum(oli => oli.ItemUnitPrice);
            }

        }
                
    }
}
