using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using PayAway.WebAPI.Shared.Entities.v1;

namespace PayAway.WebAPI.Entities.Database
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
        public int OrderId { get; set; }

        [Required]
        public Guid CatalogItemGuid { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public decimal ItemUnitPrice { get; set; }

        // Navigation Property
        public OrderDBE Order { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator CatalogItemMBE(OrderLineItemDBE from)
        {
            CatalogItemMBE to = null;

            if (from != null)
            {
                to = new CatalogItemMBE()
                {
                    ItemGuid = from.CatalogItemGuid,
                    ItemName = from.ItemName,
                    ItemUnitPrice = from.ItemUnitPrice

                };
            }
            return to;
        }

        public static explicit operator NewOrderLineItemMBE(OrderLineItemDBE from)
        {
            NewOrderLineItemMBE to = null;

            if (from != null)
            {
                to = new NewOrderLineItemMBE()
                {
                    ItemGuid = from.CatalogItemGuid
                };
            }
            return to;
        }
        #endregion
    }
}
