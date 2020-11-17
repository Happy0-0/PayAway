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
    }
}
