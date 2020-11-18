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
        [JsonPropertyName("eventDate")]
        public DateTime EventDate { get; set; }

        [JsonPropertyName("eventStatus")]
        public string EventStatus { get; set; }

        [JsonPropertyName("eventDescription")]
        public string EventDescription { get; set; }
    }
}
