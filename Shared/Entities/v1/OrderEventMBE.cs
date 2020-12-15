using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// This class contains information about an order event
    /// </summary>
    public record OrderEventMBE
    {
        [JsonPropertyName("eventDateTimeUTC")]
        public DateTime EventDateTimeUTC { get; init; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("eventStatus")]
        public Enums.ORDER_STATUS OrderStatus { get; init; }
        
        [JsonPropertyName("eventDescription")]
        public string EventDescription { get; init; }
    }
}
