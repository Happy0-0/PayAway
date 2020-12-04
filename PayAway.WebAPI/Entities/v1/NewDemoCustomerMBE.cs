using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Entities.v1
{
    /// <summary>
    /// Class represents a new or updated demo customer
    /// </summary>
    public class NewDemoCustomerMBE
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
