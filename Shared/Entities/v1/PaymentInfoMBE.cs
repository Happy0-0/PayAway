using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Shared.Entities.v1
{
    public record PaymentInfoMBE
    {
        /// <summary>
        /// Gets or sets a primary account number aka credit card number
        /// </summary>
        [JsonPropertyName("pan")]
        public string PAN { get; init; }

        /// <summary>
        /// Gets or sets the three digit code on the back of the credit card
        /// </summary>
        [JsonPropertyName("cvv")]
        public string CVV { get; init; }

        /// <summary>
        /// Gets the tip amount.
        /// </summary>
        [JsonPropertyName("tipAmount")]
        public decimal? TipAmount { get; init; }

        [JsonPropertyName("expMonth")]
        public int ExpMonth { get; init; }

        [JsonPropertyName("expYear")]
        public int ExpYear { get; init; }

    }
}
