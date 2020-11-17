using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderDBE
    {
        /// <summary>
        /// DB Generated PK for this Order.
        /// </summary>
        /// <value>The order identifier.</value>
        [Key]
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Publicly Usable Unique Identifier for this Order
        /// </summary>
        /// <value>The order unique identifier.</value>
        [Required]
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// ID of the merchant this Order is related to
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Required]
        public int MerchantId { get; set; }

        [Required]
        public DateTime OrderDateTimeUTC { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string CreditCardNumber { get; set; }

        public string AuthCode { get; set; }

        // Navigation Property
        public List<OrderEventDBE> OrderEvents { get; set; }

        // Navigation Property
        public List<OrderLineItemDBE> OrderLineItems { get; set; }

    }
}
