using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using PayAway.WebAPI.Shared.Entities.v1;

namespace PayAway.WebAPI.Entities.Database
{
    /// <summary>
    /// This class represents a "Shadow" Customer pre-setup on a Merchant to use during a demo
    /// </summary>
    public class DemoCustomerDBE
    {
        /// <summary>
        /// DB Generated PK for this Demo Customer.
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Key]
        [Required]
        public int DemoCustomerId { get; set; }

        /// <summary>
        /// Publicly Usable Unique Identifier for this Demo Customer
        /// </summary>
        /// <value>The demo customer unique identifier.</value>
        [Required]
        public Guid DemoCustomerGuid { get; set; }

        /// <summary>
        /// ID of the merchant this demo customer is related to
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Required]
        public int MerchantId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerPhoneNo { get; set; }

        // Navigation Property
        public MerchantDBE Merchant { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator DemoCustomerMBE(DemoCustomerDBE from)
        {
            DemoCustomerMBE to = null;

            if (from != null)
            {
                to = new DemoCustomerMBE()
                {
                    CustomerGuid = from.DemoCustomerGuid,
                    CustomerName = from.CustomerName,
                    CustomerPhoneNo = from.CustomerPhoneNo
                };
            }

            return to;
        }

        public static explicit operator DemoCustomerDBE(DemoCustomerMBE from)
        {
            DemoCustomerDBE to = null;

            if (from != null)
            {
                to = new DemoCustomerDBE()
                {
                    DemoCustomerGuid = from.CustomerGuid,
                    CustomerName = from.CustomerName,
                    CustomerPhoneNo = from.CustomerPhoneNo
                };
            }

            return to;
        }

        #endregion
    }
}
