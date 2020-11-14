using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class CatalogItemMBE
    {
        public Guid ItemGuid { get; set; }

        public string ItemName { get; set; }

        public decimal ItemUnitPrice { get; set; }
    }
}
