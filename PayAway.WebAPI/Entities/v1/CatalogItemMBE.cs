using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class represents an existing catalog item
    /// </summary>
    public record CatalogItemMBE
    {
        [JsonPropertyName("itemGuid")]
        public Guid ItemGuid { get; init; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; init; }

        [JsonPropertyName("itemUnitPrice")]
        public decimal ItemUnitPrice { get; init; }
    }
}
