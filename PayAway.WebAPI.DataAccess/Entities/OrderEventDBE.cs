using PayAway.WebAPI.Shared.Entities.v1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.DataAccess.Entities
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
        public Enums.ORDER_STATUS OrderStatus { get; set; }

        [Required]
        public string EventDescription { get; set; }

        // Navigation Property
        public OrderDBE Order { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator OrderEventMBE(OrderEventDBE from)
        {
            OrderEventMBE to = null;

            if(from != null)
            {
                to = new OrderEventMBE()
                {
                    EventDateTimeUTC = from.EventDateTimeUTC,
                    OrderStatus = from.OrderStatus,
                    EventDescription = from.EventDescription
                };
            }

            return to;
        }
        #endregion
    }
}
