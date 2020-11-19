using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PayAway.WebAPI.Entities.v0;

namespace PayAway.WebAPI.Entities.v1
{
    public class MerchantDBE
    {
        /// <summary>
        /// DB Generated PK for this Merchant.
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Key]
        [Required]
        public int MerchantId { get; set; }

        /// <summary>
        /// Publicly Usable Unique Identifier for this Merchant
        /// </summary>
        /// <value>The merchant unique identifier.</value>
        [Required]
        public Guid MerchantGuid { get; set; }

        public string LogoUrl { get; set; }

        [Required]
        public string MerchantName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsSupportsTips { get; set; }

        // Navigation Property
        public List<DemoCustomerDBE> DemoCustomers { get; set; }

        // Navigation Property
        public List<OrderDBE> Orders { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator MerchantMBE(MerchantDBE from)
        {
            MerchantMBE to = null;

            if (from != null)
            {
                to = new MerchantMBE()
                {
                    MerchantGuid = from.MerchantGuid,
                    MerchantName = from.MerchantName,
                    LogoUrl = from.LogoUrl,
                    IsSupportsTips = from.IsSupportsTips,
                    IsActive = from.IsActive,
                };
            }

            return to;
        }

        public static explicit operator MerchantDBE(MerchantMBE from)
        {
            MerchantDBE to = null;

            if (from != null)
            {
                to = new MerchantDBE()
                {
                    MerchantGuid = from.MerchantGuid,
                    MerchantName = from.MerchantName,
                    LogoUrl = from.LogoUrl,
                    IsSupportsTips = from.IsSupportsTips,
                    IsActive = from.IsActive,
                };
            }

            return to;
        }

        #endregion
    }
}
