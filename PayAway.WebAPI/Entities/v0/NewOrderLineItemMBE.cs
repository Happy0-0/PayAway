﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class NewOrderLineItemMBE
    {
        [JsonPropertyName("itemGuid")]
        public Guid ItemGuid { get; set; }
    }
}
