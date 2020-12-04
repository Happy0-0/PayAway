using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using PayAway.WebAPI.Controllers.v0;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class that contains summary info for an Order
    /// </summary>
    public record OrderHeaderMBE
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
        public int OrderId { get; init; }

        /// <summary>
        /// Gets or sets customers name
        /// </summary>
        /// <value>order name</value>
        [JsonPropertyName("customerName")]
        public string CustomerName { get; init; }

        /// <summary>
        /// Gets or sets the order phone number
        /// </summary>
        /// <value>phone number</value>
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; init; }

        /// <summary>
        /// Gets or sets a credit card number
        /// </summary>
        [JsonPropertyName("creditCardNumber")]
        public string PAN { get; init; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("status")]
        public Enums.ORDER_STATUS OrderStatus { get; init; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        /// <value>order total</value>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        /// <value>order date</value>
        [JsonPropertyName("orderDateTimeUTC")]
        public DateTime OrderDateTimeUTC { get; init; }
    }
}
