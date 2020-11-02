using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.v0
{
    public class NewCustomerMBE
    {
        
        /// <summary>
        /// Gets or sets the customer name
        /// </summary>
        /// <remarks>the customer name</remarks>
        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the customer phone number
        /// </summary>
        /// <value>The customer phone number</value>
        [JsonPropertyName("customerPhoneNo")]
        public string CustomerPhoneNo { get; set; }

    }
}
