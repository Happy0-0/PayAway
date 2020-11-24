using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    /// <summary>
    /// Class represents an existing Demo customer
    /// </summary>
    public class DemoCustomerMBE : NewDemoCustomerMBE
    {
        /// <summary>
        /// Gets or sets the customerID
        /// </summary>
        /// <returns>customerID</returns>
        [JsonPropertyName("customerGuid")]
        public Guid CustomerGuid { get; init; }
    }
}
