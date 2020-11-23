using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// This class contains information about order events
    /// </summary>
    public class OrderEventMBE
    {
        [JsonPropertyName("eventDateTimeUTC")]
        public DateTime EventDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets order status
        /// </summary>
        /// <value>order status</value>
        [JsonPropertyName("eventStatus")]
        public Enums.ORDER_STATUS OrderStatus { get; set; }
        
        [JsonPropertyName("eventDescription")]
        public string EventDescription { get; set; }
    }
}
