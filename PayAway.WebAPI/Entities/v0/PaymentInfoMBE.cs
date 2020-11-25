using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class PaymentInfoMBE
    {
        /// <summary>
        /// Gets or sets a primary account number aka credit card number
        /// </summary>
        [JsonPropertyName("pan")]
        public string PAN { get; set; }

        /// <summary>
        /// Gets or sets the three digit code on the back of the credit card
        /// </summary>
        [JsonPropertyName("cvv")]
        public string CVV { get; set; }

        [JsonPropertyName("tipAmount")]
        public decimal TipAmount { get; set; }

        [JsonPropertyName("expMonth")]
        public int ExpMonth { get; set; }

        [JsonPropertyName("expYear")]
        public int ExpYear { get; set; }

    }
}
