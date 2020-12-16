using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PayAway.WebAPI.Shared.Entities.v1
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "A non blank Demo Customer Name is required.")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the customer phone number
        /// </summary>
        /// <value>The customer phone number</value>
        [JsonPropertyName("customerPhoneNo")]
        [PhoneAttribute(ErrorMessage = "A valid phone number for the demo customer is required.")]
        public string CustomerPhoneNo { get; set; }

    }
}
