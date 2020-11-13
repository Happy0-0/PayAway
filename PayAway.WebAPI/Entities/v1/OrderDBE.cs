using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v1
{
    public class OrderDBE
    {
        [Key]
        [Required]
        public Guid OrderGuid { get; set; }

        public string OrderNumber { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string CreditCardNumber { get; set; }

        public string AuthCode { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public DateTime OrderDateTimeUTC { get; set; }
    }
}
