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
        [Key]
        [Required]
        public Guid MerchantID { get; set; }

        public string LogoUrl { get; set; }

        [Required]
        public string MerchantName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsSupportsTips { get; set; }

        public static explicit operator MerchantMBE(MerchantDBE from)
        {
            MerchantMBE to = null;

            if (from != null)
            {
                to = new MerchantMBE()
                {
                    MerchantID = from.MerchantID,
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
                    MerchantID = from.MerchantID.Value,
                    MerchantName = from.MerchantName,
                    LogoUrl = from.LogoUrl,
                    IsSupportsTips = from.IsSupportsTips,
                    IsActive = from.IsActive,
                };
            }

            return to;
        }
    }
}
