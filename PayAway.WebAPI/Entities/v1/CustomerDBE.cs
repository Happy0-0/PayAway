using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using PayAway.WebAPI.Entities.v0;

namespace PayAway.WebAPI.Entities.v1
{
    public class CustomerDBE
    {
        [Key]
        [Required]
        public Guid MerchantID { get; set; }

        [Required]
        public Guid CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerPhoneNo { get; set; }

        public static explicit operator CustomerMBE(CustomerDBE from)
        {
            CustomerMBE to = null;

            if (from != null)
            {
                to = new CustomerMBE()
                {
                    CustomerID = from.CustomerID,
                    CustomerName = from.CustomerName,
                    CustomerPhoneNo = from.CustomerPhoneNo
                };
            }

            return to;
        }
    }
}
