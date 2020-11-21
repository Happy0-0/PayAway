using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class CatalogItemMBE
    {
        [JsonPropertyName("itemGuid")]
        public Guid ItemGuid { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("itemUnitPrice")]
        public decimal ItemUnitPrice { get; set; }
    }
}
