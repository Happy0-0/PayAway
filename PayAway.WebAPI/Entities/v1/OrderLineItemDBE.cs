using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderLineItemDBE
    {
        /// <summary>
        /// DB Generated PK for this Order Line Item.
        /// </summary>
        /// <value>The order item identifier.</value>
        [Key]
        [Required]
        public int OrderLineItemId { get; set; }

        /// <summary>
        /// ID of the Order this Order Line Item is related to
        /// </summary>
        /// <value>The order identifier.</value>
        [Required]
        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public decimal ItemUnitPrice { get; set; }

        // Navigation Property
        public OrderDBE Order { get; set; }
    }
}
