using PayAway.WebAPI.Entities.v0;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class CatalogItemDBE
    {
        /// <summary>
        /// DB Generated PK for this Catalog Item.
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Key]
        [Required]
        public int CatalogItemId { get; set; }

        /// <summary>
        /// ID of the merchant this catalog item is related to
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Required]
        public int MerchantId{ get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public decimal ItemUnitPrice { get; set; }

        public static explicit operator CatalogItemMBE(CatalogItemDBE from)
        {
            CatalogItemMBE to = null;

            if (from != null)
            {
                to = new CatalogItemMBE()
                {
                   ItemGuid = Guid.NewGuid(),
                   ItemName = from.ItemName,
                   ItemUnitPrice = from.ItemUnitPrice

                };
            }
            return to;
        }
    }
}
