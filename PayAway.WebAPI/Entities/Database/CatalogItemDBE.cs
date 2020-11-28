using PayAway.WebAPI.Entities.v0;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.Database
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
        /// Gets or sets the catalog item unique identifier.
        /// </summary>
        /// <value>The catalog item unique identifier.</value>
        [Required]
        public Guid CatalogItemGuid { get; set; }

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

        // Navigation Property (we dop not add this because of the "default" items stored on merchantId 0
        //public MerchantDBE Merchant { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator CatalogItemMBE(CatalogItemDBE from)
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
        #endregion
    }
}
