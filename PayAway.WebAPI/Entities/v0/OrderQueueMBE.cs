﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// class that contains the order queue information
    /// </summary>
    public class OrderQueueMBE
    {
        /// <summary>
        /// Gets and sets a list of orders
        /// </summary>
        /// <returns>a list of orders</returns>
        [JsonPropertyName("orders")]
        public List<OrderHeaderMBE> Orders { get; set; }
    }
}
