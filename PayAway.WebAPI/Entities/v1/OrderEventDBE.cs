using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderEventDBE
    {
        [Key]
        [Required]
        public Guid OrderGuid { get; set; }

        [Required]
        public Guid EventGuid { get; set; }

        [Required]
        public DateTime EventDateTimeUTC { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public string EventDescription { get; set; }
    }
}
