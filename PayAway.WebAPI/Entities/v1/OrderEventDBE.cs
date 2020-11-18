﻿using PayAway.WebAPI.Entities.v0;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderEventDBE
    {
        /// <summary>
        /// DB Generated PK for this Order Event.
        /// </summary>
        /// <value>The order event identifier.</value>
        [Key]
        [Required]
        public int OrderEventId { get; set; }

        /// <summary>
        /// ID of the Order this Event is related to
        /// </summary>
        /// <value>The order identifier.</value>
        [Required]
        public int OrderId { get; set; }

        [Required]
        public DateTime EventDateTimeUTC { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public string EventDescription { get; set; }

        // Navigation Property
        public OrderDBE Order { get; set; }

        public static explicit operator OrderEventsMBE(OrderEventDBE from)
        {
            OrderEventsMBE to = null;

            if(from != null)
            {
                to = new OrderEventsMBE()
                {
                    EventDate = from.EventDateTimeUTC,
                    EventStatus = from.OrderStatus,
                    EventDescription = from.EventDescription
                };
            }

            return to;
        }
    }
}
