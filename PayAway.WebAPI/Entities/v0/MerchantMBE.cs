using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// class used when a method needs to get merchant information. Inherits from NewMerchantMBE.
    /// </summary>
    public class MerchantMBE : NewMerchantMBE
    {

        /// <summary>
        /// Gets or sets merchantID guid
        /// </summary>
        /// <value>merchantID</value>
        [JsonPropertyName("merchantID")]
        [Key]
        public Guid? MerchantID { get; set; }

        /// <summary>
        /// Gets the number of customers a merchant has
        /// </summary>
        /// <value>number of customers</value>
        [JsonPropertyName("numberOfCustomers")]
        public int? NumberOfCustomers 
        {
            get{
                if(Customers != null)
                {
                    return Customers.Count;
                }
                return null; 
            }
            
        }

        /// <summary>
        /// Gets and sets a list of customers
        /// </summary>
        /// <returns>a list of customers</returns>
        [JsonPropertyName("customers")]
        public List<CustomerMBE> Customers { get; set; }

    }
}
