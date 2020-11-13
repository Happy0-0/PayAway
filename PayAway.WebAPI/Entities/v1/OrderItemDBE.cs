﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderItemDBE
    {
        [Key]
        [Required]
        public Guid OrderGuid { get; set; }

        [Required]
        public Guid ItemGuid { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public decimal ItemUnitPrice { get; set; }
    }
}
