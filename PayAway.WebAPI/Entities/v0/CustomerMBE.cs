using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class CustomerMBE : NewCustomerMBE
    {
        /// <summary>
        /// Gets or sets the customerID
        /// </summary>
        /// <returns>customerID</returns>
        [JsonPropertyName("customerID")]
        public Guid? CustomerID { get; set; }
    }
}
