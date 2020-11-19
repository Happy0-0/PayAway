using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.Config
{
    /// <summary>
    /// This class represents the config info for a SMS WebAPI
    /// </summary>
    public class SMSServiceConfigBE
    {
        // Note: JsonPropertyName is not honored in secrets files
        public string AccountSid { get; set; }

        public string AuthToken { get; set; }

        public string PhoneNumber { get; set; }
    }
}
