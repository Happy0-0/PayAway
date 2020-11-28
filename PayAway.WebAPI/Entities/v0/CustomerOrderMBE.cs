using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class CustomerOrderMBE
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        /// <value>order number</value>
        [JsonPropertyName("orderNumber")]
        public string OrderNumber => this.OrderId.ToString("0000");

        /// <summary>
        /// Gets or sets order guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("orderGuid")]
        public Guid OrderGuid { get; init; }

        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        /// <value>order date</value>
        [JsonPropertyName("orderDateTimeUTC")]
        public DateTime OrderDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        /// <value>order total</value>
        [JsonPropertyName("orderTotal")]
        public decimal OrderTotal { get; set; }

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
        /// Gets or sets the logo url
        /// </summary>
        /// <value>The logo url</value>
        [JsonPropertyName("logoUrl")]
        public Uri LogoUrl { get; set; }

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
        /// Gets or sets a masked primary account number aka credit card number
        /// </summary>
        [JsonPropertyName("maskedPan")]
        public string MaskedPAN { get; set; }

        /// <summary>
        /// Gets or set whether or not payment is available.
        /// </summary>
        [JsonPropertyName("isPaymentAvailable")]
        public bool IsPaymentAvailable { get; set; }   
    }
}
