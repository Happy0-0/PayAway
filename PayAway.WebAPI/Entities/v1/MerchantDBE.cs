using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
