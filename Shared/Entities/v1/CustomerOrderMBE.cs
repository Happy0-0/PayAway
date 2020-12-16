using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Shared.Entities.v1
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
        /// <value>orderGuid</value>
        [JsonPropertyName("orderGuid")]
        public Guid OrderGuid { get; init; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("status")]
        public Enums.ORDER_STATUS OrderStatus { get; set; }

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
        [JsonPropertyName("orderSubTotal")]
        public decimal OrderSubTotal { get; set; }

        /// <summary>
        /// Gets or sets the tip amount.
        /// </summary>
        /// <value>The tip amount.</value>
        [JsonPropertyName("tipAmount")]
        public decimal TipAmount { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary> 
        /// <value>order total</value>
        [JsonPropertyName("orderTotal")]
        public decimal OrderTotal => this.OrderSubTotal + this.TipAmount;

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
        /// Gets or sets the name of the logo file.
        /// </summary>
        /// <value>The name of the logo file.</value>
        /// <remarks>
        /// This property is from the DB, its value is dynamically converted to LogoUrl to be returned to the front end
        /// </remarks>
        [JsonIgnore]
        public string LogoFileName { get; set; }

        /// <summary>
        /// Gets or sets the merchant url
        /// </summary>
        /// <value>The merchant website</value>
        [JsonPropertyName("merchantUrl")]
        public Uri MerchantUrl { get; set; }

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
        /// Gets or sets the authortization code.
        /// </summary>
        /// <value>The authortization code.</value>
        [JsonPropertyName("authCode")]
        public string AuthortizationCode { get; set; }

        /// <summary>
        /// Gets or set whether or not payment is available.
        /// </summary>
        [JsonPropertyName("isPaymentAvailable")]
        public bool IsPaymentAvailable => this.OrderStatus != Enums.ORDER_STATUS.Paid;
    }
}
